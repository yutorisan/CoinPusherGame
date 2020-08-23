using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility;

namespace MedalPusher {
	public interface IPusherMove {
		/// <summary>
		/// Pusherを動作させます。
		/// </summary>
		void StartMove();
		/// <summary>
		/// Pusherを停止します。
		/// </summary>
		void StopMove();
		/// <summary>
		/// 移動周期(s)
		/// </summary>
		float MoveCycle { get; set; }
		/// <summary>
		/// 移動の長さ
		/// </summary>
		float MoveLength { get; set; }
	}

	public class PusherMove : MonoBehaviour, IPusherMove
	{
		#region MonoBehaviour
		// Start is called before the first frame update
		void Start()
		{
			MoveCycle = 6.0f;
			MoveLength = 0.2f;

			m_rigidbody = this.GetComponent<Rigidbody>();
		}

		// Update is called once per frame
		void Update()
		{
			if(IsMoving) {
				float elapsedTime = m_stopTime + (Time.time - m_startTIme); //ストップ後に同じ位置から再開するため、ストップした時の値を保持
				double degree = 360 * (elapsedTime % MoveCycle) / MoveCycle;
				m_nowPositionX = MoveLength * Mathf.Sin((float)degree.ToRadian());
				
				m_rigidbody.MovePosition(new Vector3(m_nowPositionX ,
												   transform.position.y,
												   transform.position.z));
			}
		}
		#endregion

		public void StartMove() {
			m_startTIme = Time.time;
			IsMoving = true;
		}

		public void StopMove() {
			m_stopTime = Time.time;
			IsMoving = false;
		}

		public float MoveCycle { get; set; }
		public float MoveLength { get; set; }
		public bool IsMoving { get; private set; }

		private float m_nowPositionX;
		private float m_startTIme;
		private float m_stopTime;

		private Rigidbody m_rigidbody;

	}

}
