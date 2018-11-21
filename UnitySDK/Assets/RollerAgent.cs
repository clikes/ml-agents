using System.Collections;
using System.Collections.Generic;
using MLAgents;
using UnityEngine;
public class RollerAgent : Agent {

	Rigidbody rBody;
    public float speed;
    float previousDistance = float.MaxValue;

    void Start () {
		rBody = GetComponent<Rigidbody> ();
	}

	public Transform Target;
	public override void AgentReset () {
		if (this.transform.position.y < -1.0) {
			// agent 掉落
			this.transform.position = Vector3.zero;
			this.rBody.angularVelocity = Vector3.zero;
			this.rBody.velocity = Vector3.zero;
		} else {
			// 将目标移动到新的位置
			Target.position = new Vector3 (Random.value * 8 - 4,
				0.5f,
				Random.value * 8 - 4);
		}
        previousDistance = Vector3.Distance(this.transform.position,
                                          Target.position);

    }
	///状态观测
	List<float> observation = new List<float> ();
	public override void CollectObservations () {
		// 计算相对位置
		Vector3 relativePosition = Target.position - this.transform.position;

		// 相对位置
		AddVectorObs (relativePosition.x / 5);
		AddVectorObs (relativePosition.z / 5);

		// 与平台边缘的距离
		AddVectorObs ((this.transform.position.x + 5) / 5);
		AddVectorObs ((this.transform.position.x - 5) / 5);
		AddVectorObs ((this.transform.position.z + 5) / 5);
		AddVectorObs ((this.transform.position.z - 5) / 5);

		// Agent 速度
		AddVectorObs (rBody.velocity.x / 5);
		AddVectorObs (rBody.velocity.z / 5);
	}

    public override void AgentAction(float[] vectorAction, string textAction)
    {

        float distanceToTarget = Vector3.Distance(this.transform.position,
                                          Target.position);
        // 时间惩罚
        AddReward(-0.05f);

        // 已到达目标
        if (distanceToTarget < 1.42f)
        {
            Done();
            AddReward(1.0f);
        }

        // 进一步接近
        if (distanceToTarget < previousDistance)
        {
            AddReward(0.1f);
        }

        // 掉下平台
        if (this.transform.position.y < -1.0)
        {
            Done();
            AddReward(-1.0f);
        }
        previousDistance = distanceToTarget;

        
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = Mathf.Clamp(vectorAction[0], -1, 1);//钳制value 大于1 则返回1 小于-1则返回-1
        controlSignal.z = Mathf.Clamp(vectorAction[1], -1, 1);
        rBody.AddForce(controlSignal * speed);
    }

    // Update is called once per frame
    void Update () {

	}
}