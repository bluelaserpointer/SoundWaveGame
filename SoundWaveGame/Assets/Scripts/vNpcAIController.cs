using UnityEngine;
using UnityEngine.AI;

namespace Invector.vCharacterController
{

    [RequireComponent(typeof(NavMeshAgent))]
    public class vNpcAIController : vThirdPersonController
    {
        public enum NpcState { Idle, Chase }

        [Header("AI")]
        public NpcState state = NpcState.Idle;
        [Tooltip("到达目标点后视为到达的停止距离（会同步到 NavMeshAgent.stoppingDistance）")]
        public float stopDistance = 0.2f;
        [Tooltip("目标很近时减速的距离，用于更平滑地停下")]
        public float slowDownDistance = 1.0f;

        private NavMeshAgent agent;
        private Vector3? currentTarget;

        void Start()
        {
            animator = GetComponentInChildren<Animator>();
            agent = GetComponent<NavMeshAgent>();
            agent.updateRotation = false;   // 旋转交给父类（动画/旋转系统）
            agent.updatePosition = true;    // 位置由NavMesh驱动

            agent.stoppingDistance = stopDistance;

            // AI通常不使用RootMotion位移（仍可用动画根运动做细节），这里关闭以NavMesh为准
            useRootMotion = false;
            isGrounded = true;              // 敌人一般在地面，确保 Animator 有正确的 Grounded
        }

        private void Update()
        {
            // 1) 状态机：目标与Agent
            TickStateMachine();

            // 2) 同步 NavMesh 速度到父类输入系（input / moveDirection）
            UpdateInputFromAgent();

            // 3) 选择当前速度预设（与父类一致：free/strafe）
            vMovementSpeed speedPreset = isStrafing ? strafeSpeed : freeSpeed;

            // 4) 影响动画的标志位（停/走），这些会被 UpdateAnimator 使用
            stopMove = (state == NpcState.Idle) || agent.isStopped || agent.velocity.sqrMagnitude < 0.0001f;
            isGrounded = true; // 若你有实际落地检测，可以替换为真实值

            // 5) —— 核心补充 —— 驱动 Animator
            // 先根据 moveDirection 计算 vertical/horizontal/inputMagnitude
            SetAnimatorMoveSpeed(speedPreset);
            // 再把一系列参数写入 Animator（包括 Input(H/V)、Magnitude、Strafe/Sprint/Grounded 等）
            UpdateAnimator();

            // 6) 旋转与根运动控制（即便 useRootMotion=false，也让父类处理转向/根位姿同步）
            ControlRotationType();
            //ControlAnimatorRootMotion();
        }

        /// <summary>
        /// 外部调用：通知去某个世界坐标（由“视野/感知系统”或“上层AI”来调用）
        /// </summary>
        public void NotifyMoveTo(Vector3 worldPos)
        {
            currentTarget = worldPos;
            state = NpcState.Chase;
            if (agent.enabled)
            {
                agent.isStopped = false;
                agent.SetDestination(worldPos);
            }
        }

        /// <summary>
        /// （可选）外部调用：强制停下/清除目标
        /// </summary>
        public void ClearTargetAndStop()
        {
            currentTarget = null;
            if (agent.enabled)
            {
                agent.ResetPath();
                agent.isStopped = true;
            }
            state = NpcState.Idle;
        }

        private void TickStateMachine()
        {
            switch (state)
            {
                case NpcState.Idle:
                    // 没有目标或已到达：确保停下
                    if (currentTarget.HasValue)
                    {
                        // 若还保留目标但被置为Idle，重新切到Chase
                        state = NpcState.Chase;
                    }
                    else
                    {
                        if (agent.enabled && !agent.isStopped) agent.isStopped = true;
                    }
                    break;

                case NpcState.Chase:
                    if (!currentTarget.HasValue)
                    {
                        state = NpcState.Idle;
                        break;
                    }

                    if (agent.enabled)
                    {
                        // 如果路径失效，尝试重设
                        if (!agent.hasPath || agent.pathStatus == NavMeshPathStatus.PathInvalid)
                        {
                            agent.SetDestination(currentTarget.Value);
                        }

                        // 到达判定
                        if (!agent.pathPending)
                        {
                            float remain = agent.remainingDistance;
                            if (remain <= Mathf.Max(agent.stoppingDistance, stopDistance))
                            {
                                // 视为到达：进入Idle并清空目标
                                ClearTargetAndStop();
                            }
                            else
                            {
                                // 根据父类移动参数，设置NavMeshAgent速度上限
                                ApplySpeedLimit(remain);
                            }
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// 将NavMeshAgent的平面速度/期望速度映射到父类的输入系（input / moveDirection）
        /// 以便继承的动画控制器能够正确Blend。
        /// </summary>
        private void UpdateInputFromAgent()
        {
            // 期望使用的“目标速度”（走 or 跑），和父类保持一致
            float targetMaxSpeed = GetDesiredMaxSpeedForCurrentLocomotion();

            Vector3 worldVel = agent.velocity;
            Vector3 planarVel = new Vector3(worldVel.x, 0f, worldVel.z);
            float speed = planarVel.magnitude;

            // 归一化到 [-1, 1] 输入空间（父类通常以输入幅度控制动画Blend）
            Vector3 dir = speed > 0.001f ? (planarVel / speed) : Vector3.zero;
            float inputMagnitude = (targetMaxSpeed > 0.01f) ? Mathf.Clamp01(speed / targetMaxSpeed) : 0f;

            // input 是角色局部系方向，所以把世界方向转到局部
            Vector3 localDir = transform.InverseTransformDirection(dir) * inputMagnitude;

            // ---- 同步到父类变量 ----
            // 说明：这些字段在 Invector 的父层通常是 protected；若你当前工程里是 private，
            // 可以改成 protected，或在本类里提供 SetInput 的受保护接口后再设置。
            input = new Vector3(localDir.x, 0f, localDir.z);

            // 更新 moveDirection 给父类的旋转/位移计算用（世界空间方向）
            // 父类会在 ControlRotationType / UpdateMoveDirection 中用到 inputSmooth；
            // 这里直接给一个“朝向速度”的世界方向，便于父类转向
            moveDirection = dir; // 世界向前方向（不乘幅度，幅度通过 input 控制）

            // 站立/Idle时，让输入与方向平滑归零（与 vMovementSpeed.movementSmooth 一致）
            if (state == NpcState.Idle || agent.isStopped || speed < 0.01f)
            {
                float smooth = (isStrafing ? strafeSpeed.movementSmooth : freeSpeed.movementSmooth);
                input = Vector3.Lerp(input, Vector3.zero, smooth * Time.deltaTime);
                moveDirection = Vector3.Lerp(moveDirection, Vector3.zero, smooth * Time.deltaTime);
            }
        }

        /// <summary>
        /// 根据父类的速度设定，给 NavMeshAgent 应用速度上限（走/跑），并做接近目标时的减速。
        /// </summary>
        private void ApplySpeedLimit(float remainingDistance)
        {
            float maxSpeed = GetDesiredMaxSpeedForCurrentLocomotion();

            // 接近目标时做线性减速，避免急停
            if (remainingDistance <= slowDownDistance)
            {
                float t = Mathf.Clamp01(remainingDistance / Mathf.Max(0.001f, slowDownDistance));
                agent.speed = Mathf.Max(0.1f, maxSpeed * t);
            }
            else
            {
                agent.speed = maxSpeed;
            }

            // 与父类旋转速度保持一致一点点（可选）
            agent.angularSpeed = Mathf.Max(agent.angularSpeed, (isStrafing ? strafeSpeed.rotationSpeed : freeSpeed.rotationSpeed) * 10f);
            agent.acceleration = Mathf.Max(agent.acceleration, maxSpeed * 4f);
        }

        /// <summary>
        /// 依据父类的移动模式，选择“走”还是“跑”的目标最大速度。
        /// （本版本不写“冲刺/奔跑”的触发逻辑）
        /// </summary>
        private float GetDesiredMaxSpeedForCurrentLocomotion()
        {
            vMovementSpeed speedPreset = isStrafing ? strafeSpeed : freeSpeed;

            // 若 walkByDefault 为 true，则以 walk 速度为主；否则用 running 速度
            float desired = speedPreset.walkByDefault ? speedPreset.walkSpeed : speedPreset.runningSpeed;

            // 若你未来加入奔跑逻辑（isSprinting），这里再覆盖：
            // if (isSprinting) desired = speedPreset.sprintSpeed;

            return Mathf.Max(0.01f, desired);
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (currentTarget.HasValue)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(currentTarget.Value, 0.2f);
            }
        }
#endif
    }
}
