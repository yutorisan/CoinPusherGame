using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace MedalPusher.Pusher {

	public class PusherManager : MonoBehaviour {
		private void Awake() { 
			m_pusherMove = GameObject.Find("PusherPlate").GetComponent<IPusherMove>();

			m_move.Subscribe(check => {
				if(check) {
					m_pusherMove.StartMove();
				} else {
					m_pusherMove.StopMove();
				}
			}).AddTo(this);
		}

		[SerializeField]
        public BoolReactiveProperty m_move = new BoolReactiveProperty(true);
		private IPusherMove m_pusherMove;
	}

}
