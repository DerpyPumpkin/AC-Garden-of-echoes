using UnityEngine;
using Spine;
using Spine.Unity;

namespace AC.Spine
{

	public class DirectionMapper : MonoBehaviour
	{

		#region Variables

		[HideInInspector] public SkeletonAnimation skeletonAnimation = null;
		[HideInInspector] public SkeletonDataAsset[] skeletonDataAssets = new SkeletonDataAsset[0];

		private string currentDirection;
		private Char character;

		#endregion


		#region PublicFunctions

		public void PlayClip(string clip, int trackIndex, bool doLoop)
		{
			if (skeletonAnimation != null && skeletonAnimation.state != null)
			{
				TrackEntry track = skeletonAnimation.state.GetCurrent(trackIndex);
				if (track != null && track.Animation.Name == clip)
				{
					return;
				}

				skeletonAnimation.state.SetAnimation(trackIndex, clip, doLoop);
			}
		}


		public void SetCurrentDirection(string newDirection)
		{
			if (currentDirection != newDirection)
			{
				currentDirection = newDirection;

				if (character == null)
				{
					character = GetComponent<Char>();
				}

				SpriteDirectionData.SpriteDirection[] spriteDirections = character.spriteDirectionData.SpriteDirections;
				if (spriteDirections.Length == 0)
				{
					SetSkeleteonDataAsset(skeletonDataAssets[0]);
				}
				else
				{
					for (int i = 0; i < spriteDirections.Length; i++)
					{
						if (currentDirection == ("_" + spriteDirections[i].suffix))
						{
							SetSkeleteonDataAsset(skeletonDataAssets[i]);
							break;
						}
					}
				}
				skeletonAnimation.Initialize(true);
			}
		}

		#endregion


		#region PrivateFunctions

		private void SetSkeleteonDataAsset(SkeletonDataAsset skeletonDataAsset)
		{
			if (skeletonDataAsset != null)
			{
				skeletonAnimation.skeletonDataAsset = skeletonDataAsset;
			}
		}

		#endregion

	}

}