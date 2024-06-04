using UnityEngine;
using Spine;
using Spine.Unity;

namespace AC.Spine
{

	public class CompletionListener : MonoBehaviour
	{

		#region Variables

		private string waitingAnimation;
		private SkeletonAnimation skeletonAnimation;
		private SkeletonGraphic skeletonGraphic;

		#endregion


		#region UnityStandards

		private void OnEnable()
		{
			DirectionMapper spineMapper = GetComponent<DirectionMapper>();
			if (spineMapper != null)
			{
				skeletonAnimation = spineMapper.skeletonAnimation;
			}

			if (skeletonAnimation == null)
			{
				skeletonAnimation = GetComponent<SkeletonAnimation>();
			}

			if (skeletonAnimation == null && skeletonGraphic == null)
			{
				skeletonGraphic = GetComponent<SkeletonGraphic>();
			}

			if (skeletonAnimation)
			{
				skeletonAnimation.state.Complete += OnCompleteAnimation;
			}
			else if (skeletonGraphic)
			{
				skeletonGraphic.AnimationState.Complete += OnCompleteAnimation;
			}
		}


		private void OnDisable()
		{
			if (skeletonAnimation)
			{
				skeletonAnimation.state.Complete -= OnCompleteAnimation;
			}
			else if (skeletonGraphic)
			{
				skeletonGraphic.AnimationState.Complete -= OnCompleteAnimation;
			}
		}

		#endregion


		#region PublicFunctions

		public void SetWaitingAnimation(string clip)
		{
			if (skeletonAnimation)
			{
				skeletonAnimation.state.Complete -= OnCompleteAnimation;
				skeletonAnimation.state.Complete += OnCompleteAnimation;
			}
			else if (skeletonGraphic)
			{
				skeletonGraphic.AnimationState.Complete -= OnCompleteAnimation;
				skeletonGraphic.AnimationState.Complete += OnCompleteAnimation;
			}

			waitingAnimation = clip;
		}


		public bool IsWaiting()
		{
			return waitingAnimation != string.Empty;
		}

		#endregion


		#region PrivateFunctions

		private void OnCompleteAnimation(TrackEntry trackEntry)
		{
			if (trackEntry.Animation.Name == waitingAnimation)
			{
				waitingAnimation = string.Empty;
			}
		}

		#endregion

	}

}