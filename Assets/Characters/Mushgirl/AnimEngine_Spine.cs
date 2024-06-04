using UnityEngine;
using System.Collections.Generic;
using Spine;
using Spine.Unity;

#if UNITY_EDITOR
using UnityEditor;
#endif

using AC.Spine;

namespace AC
{

	public class AnimEngine_Spine : AnimEngine
	{
#if UNITY_EDITOR
		private static bool listExpectedAnimations = false;
#endif

		private string hideHeadClip = "HideHead";


		public override void Declare(AC.Char _character)
		{
			character = _character;
			turningStyle = TurningStyle.Linear;
			isSpriteBased = true;
			updateHeadAlways = true;
		}


		public override PlayerData SavePlayerData(PlayerData playerData, Player player)
		{
			playerData.playerIdleAnim = player.idleAnimSprite;
			playerData.playerWalkAnim = player.walkAnimSprite;
			playerData.playerRunAnim = player.runAnimSprite;
			playerData.playerTalkAnim = player.talkAnimSprite;

			return playerData;
		}


		public override void LoadPlayerData(PlayerData playerData, Player player)
		{
			player.idleAnimSprite = playerData.playerIdleAnim;
			player.walkAnimSprite = playerData.playerWalkAnim;
			player.talkAnimSprite = playerData.playerTalkAnim;
			player.runAnimSprite = playerData.playerRunAnim;
		}


		public override NPCData SaveNPCData(NPCData npcData, NPC npc)
		{
			npcData.idleAnim = npc.idleAnimSprite;
			npcData.walkAnim = npc.walkAnimSprite;
			npcData.talkAnim = npc.talkAnimSprite;
			npcData.runAnim = npc.runAnimSprite;

			return npcData;
		}


		public override void LoadNPCData(NPCData npcData, NPC npc)
		{
			npc.idleAnimSprite = npcData.idleAnim;
			npc.walkAnimSprite = npcData.walkAnim;
			npc.talkAnimSprite = npcData.talkAnim;
			npc.runAnimSprite = npcData.runAnim;
		}


#if UNITY_EDITOR

		private string ShowExpected(AC.Char character)
		{
			string result = "\n";

			if (!character.spriteDirectionData.HasDirections())
			{
				result += "\n";
				if (!string.IsNullOrEmpty(character.idleAnimSprite)) result += character.idleAnimSprite + "   (0)\n";
				if (!string.IsNullOrEmpty(character.walkAnimSprite)) result += character.walkAnimSprite + "   (0)\n";
				if (!string.IsNullOrEmpty(character.runAnimSprite)) result += character.runAnimSprite + "   (0)\n";

				if (character.separateTalkingLayer)
				{
					if (!string.IsNullOrEmpty(character.idleAnimSprite)) result += character.idleAnimSprite + "   (" + character.headLayer.ToString() + ")\n";
					if (!string.IsNullOrEmpty(character.talkAnimSprite)) result += character.talkAnimSprite + "   (" + character.headLayer.ToString() + ")\n";
					if (!string.IsNullOrEmpty(hideHeadClip)) result += hideHeadClip + "   (" + character.headLayer.ToString() + ")\n";
				}
				else
				{
					if (!string.IsNullOrEmpty(character.talkAnimSprite)) result += character.talkAnimSprite + "   (0)\n";
				}

				return result;
			}

			SpriteDirectionData.SpriteDirection[] spriteDirections = character.spriteDirectionData.SpriteDirections;
			for (int i = 0; i < spriteDirections.Length; i++)
			{
				if (character.frameFlipping == AC_2DFrameFlipping.LeftMirrorsRight && spriteDirections[i].suffix.StartsWith("L"))
				{
					continue;
				}

				if (character.frameFlipping == AC_2DFrameFlipping.RightMirrorsLeft && spriteDirections[i].suffix.StartsWith("R"))
				{
					continue;
				}

				result += "\nFor Skeleton data (" + spriteDirections[i].suffix + "):\n";

				if (!string.IsNullOrEmpty(character.idleAnimSprite)) result += character.idleAnimSprite + "   (0)\n";
				if (!string.IsNullOrEmpty(character.walkAnimSprite)) result += character.walkAnimSprite + "   (0)\n";
				if (!string.IsNullOrEmpty(character.runAnimSprite)) result += character.runAnimSprite + "   (0)\n";

				if (character.separateTalkingLayer)
				{
					if (!string.IsNullOrEmpty(character.idleAnimSprite)) result += character.idleAnimSprite + "   (" + character.headLayer.ToString() + ")\n";
					if (!string.IsNullOrEmpty(character.talkAnimSprite)) result += character.talkAnimSprite + "   (" + character.headLayer.ToString() + ")\n";
					if (!string.IsNullOrEmpty(hideHeadClip)) result += hideHeadClip + "   (" + character.headLayer.ToString() + ")\n";
				}
				else
				{
					if (!string.IsNullOrEmpty(character.talkAnimSprite)) result += character.talkAnimSprite + "   (0)\n";
				}
			}

			return result;

		}

#endif


		public override void CharSettingsGUI()
		{
#if UNITY_EDITOR

			EditorGUILayout.BeginVertical("Button");
			EditorGUILayout.LabelField("Standard 2D animations", EditorStyles.boldLabel);

			character.spriteChild = (Transform)CustomGUILayout.ObjectField<Transform>("Sprite child:", character.spriteChild, true, "", "The sprite Transform, which should be a child GameObject");
			character.idleAnimSprite = CustomGUILayout.TextField("Idle name:", character.idleAnimSprite, "", "The name of the 'Idle' animation(s), without suffix");
			character.walkAnimSprite = CustomGUILayout.TextField("Walk name:", character.walkAnimSprite, "", "The name of the 'Walk' animation(s), without suffix");
			character.runAnimSprite = CustomGUILayout.TextField("Run name:", character.runAnimSprite, "", "The name of the 'Run' animation(s), without suffix");
			character.talkAnimSprite = CustomGUILayout.TextField("Talk name:", character.talkAnimSprite, "", "The name of the 'Talk' animation(s), without suffix");
			character.separateTalkingLayer = CustomGUILayout.Toggle("Head on separate layer?", character.separateTalkingLayer, "", "If True, the head animation will be handled on a non-root layer when talking");

			if (character.separateTalkingLayer)
			{
				character.headLayer = CustomGUILayout.IntField("Head track:", character.headLayer, "", "The track index to play head animations while talking");
				if (character.headLayer < 1)
				{
					EditorGUILayout.HelpBox("The head track index must be 1 or greater.", MessageType.Warning);
				}
			}

			character.spriteDirectionData.ShowGUI();
			character.angleSnapping = AngleSnapping.None;

			if (character.spriteDirectionData.HasDirections())
			{
				character.frameFlipping = (AC_2DFrameFlipping)CustomGUILayout.EnumPopup("Frame flipping:", character.frameFlipping, "", "The type of frame-flipping to use");
				if (character.frameFlipping != AC_2DFrameFlipping.None)
				{
					character.flipCustomAnims = CustomGUILayout.Toggle("Flip custom animations?", character.flipCustomAnims, "", "If True, then custom animations will also be flipped");
				}
			}

			UnityVersionHandler.AddComponentToGameObject<DirectionMapper>(character.gameObject);

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(" ", GUILayout.Width(9));
			listExpectedAnimations = EditorGUILayout.Foldout(listExpectedAnimations, "List expected animations?");
			EditorGUILayout.EndHorizontal();

			if (listExpectedAnimations)
			{
				string result = ShowExpected(character);
				EditorGUILayout.HelpBox("The following animations are required, based on the settings above:" + result, MessageType.Info);
			}

			EditorGUILayout.EndVertical();

			if (DirectionMapper != null)
			{
				EditorGUILayout.BeginVertical("Button");
				EditorGUILayout.LabelField("Spine setup", EditorStyles.boldLabel);
				DirectionMapper.skeletonAnimation = (SkeletonAnimation)EditorGUILayout.ObjectField("Skeleton animation:", DirectionMapper.skeletonAnimation, typeof(SkeletonAnimation), true);

				SpriteDirectionData.SpriteDirection[] spriteDirections = character.spriteDirectionData.SpriteDirections;
				if (spriteDirections != null)
				{
					int newLength = Mathf.Max(1, spriteDirections.Length);
					if (DirectionMapper.skeletonDataAssets.Length != newLength)
					{
						DirectionMapper.skeletonDataAssets = new SkeletonDataAsset[newLength];
					}

					for (int i = 0; i < newLength; i++)
					{
						string label = string.Empty;
						if (i < spriteDirections.Length)
						{
							label = " (" + spriteDirections[i].suffix + ")";

							if (character.frameFlipping == AC_2DFrameFlipping.LeftMirrorsRight && spriteDirections[i].suffix.StartsWith("L"))
							{
								continue;
							}

							if (character.frameFlipping == AC_2DFrameFlipping.RightMirrorsLeft && spriteDirections[i].suffix.StartsWith("R"))
							{
								continue;
							}
						}

						DirectionMapper.skeletonDataAssets[i] = (SkeletonDataAsset)EditorGUILayout.ObjectField("Skeleton data" + label + ":", DirectionMapper.skeletonDataAssets[i], typeof(SkeletonDataAsset), false);
					}
				}
				EditorGUILayout.EndVertical();
			}

			if (GUI.changed)
			{
				EditorUtility.SetDirty(character);
				EditorUtility.SetDirty(DirectionMapper);
			}

#endif

		}


		public override void ActionCharAnimGUI(ActionCharAnim action, List<ActionParameter> parameters = null)
		{
#if UNITY_EDITOR
			action.method = (ActionCharAnim.AnimMethodChar)EditorGUILayout.EnumPopup("Method:", action.method);
			if (action.method == ActionCharAnim.AnimMethodChar.PlayCustom)
			{
				action.clip2DParameterID = Action.ChooseParameterGUI("Clip:", parameters, action.clip2DParameterID, ParameterType.String);
				if (action.clip2DParameterID < 0)
				{
					action.clip2D = EditorGUILayout.TextField("Clip:", action.clip2D);
				}

				action.includeDirection = EditorGUILayout.Toggle("Add directional suffix?", action.includeDirection);

				if (action.animChar != null && action.animChar.separateTalkingLayer)
				{
					action.hideHead = EditorGUILayout.Toggle("Hide head?", action.hideHead);
					if (action.hideHead)
					{
						EditorGUILayout.HelpBox("The head layer will play '" + hideHeadClip + "' for the duration.", MessageType.Info);
					}
				}

				action.layerInt = EditorGUILayout.IntField("Track index:", action.layerInt);
				action.doLoop = EditorGUILayout.Toggle("Loop?", action.doLoop);
				if (!action.doLoop)
				{
					action.willWait = EditorGUILayout.Toggle("Wait until finish?", action.willWait);
					if (action.willWait)
					{
						action.idleAfter = EditorGUILayout.Toggle("Return to idle after?", action.idleAfter);
					}
				}
			}
			else if (action.method == ActionCharAnim.AnimMethodChar.StopCustom)
			{
				EditorGUILayout.HelpBox("This Action does not work for Sprite-based characters.", MessageType.Info);
			}
			else if (action.method == ActionCharAnim.AnimMethodChar.SetStandard)
			{
				action.standard = (AnimStandard)EditorGUILayout.EnumPopup("Change:", action.standard);
				action.clip2DParameterID = Action.ChooseParameterGUI("Clip:", parameters, action.clip2DParameterID, ParameterType.String);
				if (action.clip2DParameterID < 0)
				{
					action.clip2D = EditorGUILayout.TextField("Clip:", action.clip2D);
				}

				if (action.standard == AnimStandard.Walk || action.standard == AnimStandard.Run)
				{
					action.changeSound = EditorGUILayout.Toggle("Change sound?", action.changeSound);
					if (action.changeSound)
					{
						action.newSoundParameterID = Action.ChooseParameterGUI("New sound:", parameters, action.newSoundParameterID, ParameterType.UnityObject);
						if (action.newSoundParameterID < 0)
						{
							action.newSound = (AudioClip)EditorGUILayout.ObjectField("New sound:", action.newSound, typeof(AudioClip), false);
						}
					}

					action.changeSpeed = EditorGUILayout.Toggle("Change speed?", action.changeSpeed);
					if (action.changeSpeed)
					{
						action.newSpeed = EditorGUILayout.FloatField("New speed:", action.newSpeed);
					}
				}
			}

			if (GUI.changed)
			{
				EditorUtility.SetDirty(action);
			}

#endif

		}


		public override float ActionCharAnimRun(ActionCharAnim action)
		{
			string clip2DNew = action.clip2D;
			if (action.includeDirection)
			{
				clip2DNew += character.GetSpriteDirection();
			}

			if (!action.isRunning)
			{
				action.isRunning = true;

				if (action.method == ActionCharAnim.AnimMethodChar.PlayCustom && !string.IsNullOrEmpty(action.clip2D))
				{
					character.charState = CharState.Custom;
					PlayCharAnim(clip2DNew, action.layerInt, action.doLoop);

					if (action.hideHead && character.separateTalkingLayer)
					{
						PlayHeadAnim(hideHeadClip);
					}

					if (action.willWait && !action.doLoop)
					{
						CompletionListener completionListener = DirectionMapper.GetComponent<CompletionListener>();
						if (completionListener == null)
						{
							completionListener = DirectionMapper.gameObject.AddComponent<CompletionListener>();
						}

						completionListener.SetWaitingAnimation(clip2DNew);
						return action.defaultPauseTime;
					}
				}
				else if (action.method == ActionCharAnim.AnimMethodChar.ResetToIdle)
				{
					character.charState = CharState.Idle;
				}
				else if (action.method == ActionCharAnim.AnimMethodChar.SetStandard)
				{
					if (action.clip2D != "")
					{
						if (action.standard == AnimStandard.Idle)
						{
							character.idleAnimSprite = action.clip2D;
						}
						else if (action.standard == AnimStandard.Walk)
						{
							character.walkAnimSprite = action.clip2D;
						}
						else if (action.standard == AnimStandard.Talk)
						{
							character.talkAnimSprite = action.clip2D;
						}
						else if (action.standard == AnimStandard.Run)
						{
							character.runAnimSprite = action.clip2D;
						}
					}

					if (action.changeSpeed)
					{
						if (action.standard == AnimStandard.Walk)
						{
							character.walkSpeedScale = action.newSpeed;
						}
						else if (action.standard == AnimStandard.Run)
						{
							character.runSpeedScale = action.newSpeed;
						}
					}

					if (action.changeSound)
					{
						if (action.standard == AnimStandard.Walk)
						{
							if (action.newSound != null)
							{
								character.walkSound = action.newSound;
							}
							else
							{
								character.walkSound = null;
							}
						}
						else if (action.standard == AnimStandard.Run)
						{
							if (action.newSound != null)
							{
								character.runSound = action.newSound;
							}
							else
							{
								character.runSound = null;
							}
						}
					}
				}
			}
			else
			{
				CompletionListener completionListener = DirectionMapper.GetComponent<CompletionListener>();
				if (!completionListener.IsWaiting())
				{
					if (action.idleAfter)
					{
						character.charState = CharState.Idle;
					}

					action.isRunning = false;
					return 0f;
				}

				return action.defaultPauseTime;
			}

			return 0f;
		}


		public override void ActionCharAnimSkip(ActionCharAnim action)
		{
			if (action.method == ActionCharAnim.AnimMethodChar.SetStandard)
			{
				ActionCharAnimRun(action);
				return;
			}
			else if (action.method == ActionCharAnim.AnimMethodChar.ResetToIdle)
			{
				character.charState = CharState.Idle;
				return;
			}

			string clip2DNew = action.clip2D;
			if (action.includeDirection)
			{
				clip2DNew += character.GetSpriteDirection();
			}

			if (action.method == ActionCharAnim.AnimMethodChar.PlayCustom)
			{
				if (action.willWait && action.idleAfter)
				{
					character.charState = CharState.Idle;
				}
				else
				{
					character.charState = CharState.Custom;
					PlayCharAnim(clip2DNew, action.layerInt);
				}
			}
		}


		public override void ActionAnimGUI(ActionAnim action, List<ActionParameter> parameters)
		{
#if UNITY_EDITOR
			action.method = (AnimMethod)EditorGUILayout.EnumPopup("Method:", action.method);
			if (action.method == AnimMethod.PlayCustom)
			{
				action.parameterID = AC.Action.ChooseParameterGUI("Skeleton Animation:", parameters, action.parameterID, ParameterType.GameObject);
				if (action.parameterID >= 0)
				{
					action.constantID = 0;
					action._anim2D = null;
				}
				else
				{
					action._anim2D = (Transform)EditorGUILayout.ObjectField("Skeleton Animation:", action._anim2D, typeof(Transform), true);

					action.constantID = action.FieldToID(action._anim2D, action.constantID);
					action._anim2D = action.IDToField(action._anim2D, action.constantID, false);
				}

				action.clip2DParameterID = Action.ChooseParameterGUI("Clip:", parameters, action.clip2DParameterID, ParameterType.String);
				if (action.clip2DParameterID < 0)
				{
					action.clip2D = EditorGUILayout.TextField("Clip:", action.clip2D);
				}

				action.layerInt = EditorGUILayout.IntField("Track index:", action.layerInt);
				bool doLoop = (action.wrapMode2D == ActionAnim.WrapMode2D.Loop);
				doLoop = EditorGUILayout.Toggle("Loop?", doLoop);
				action.wrapMode2D = (doLoop) ? ActionAnim.WrapMode2D.Loop : ActionAnim.WrapMode2D.Once;

				if (!doLoop)
				{
					action.willWait = EditorGUILayout.Toggle("Wait until finish?", action.willWait);
				}
			}
			else if (action.method == AnimMethod.StopCustom)
			{
				EditorGUILayout.HelpBox("'Stop Custom' is not available for Unity-based 2D animation.", MessageType.Info);
			}
			else if (action.method == AnimMethod.BlendShape)
			{
				EditorGUILayout.HelpBox("BlendShapes are not available in 2D animation.", MessageType.Info);
			}

			if (GUI.changed)
			{
				EditorUtility.SetDirty(action);
			}

#endif
		}


		public override string ActionAnimLabel(ActionAnim action)
		{
			if (action._anim2D)
			{
				string label = action._anim2D.name;

				if (action.method == AnimMethod.PlayCustom && !string.IsNullOrEmpty(action.clip2D))
				{
					label += " - " + action.clip2D;
				}
				return label;
			}

			return string.Empty;
		}


		public override void ActionAnimAssignValues(ActionAnim action, List<ActionParameter> parameters)
		{
			action.runtimeAnim2D = action.AssignFile(parameters, action.parameterID, action.constantID, action._anim2D);
		}


		public override float ActionAnimRun(ActionAnim action)
		{
			if (!action.isRunning)
			{
				action.isRunning = true;

				if (action.runtimeAnim2D != null && !string.IsNullOrEmpty(action.clip2D))
				{
					if (action.method == AnimMethod.PlayCustom)
					{
						SkeletonAnimation skeletonAnimation = action.runtimeAnim2D.GetComponent<SkeletonAnimation>();
						if (skeletonAnimation)
						{
							SetAnimation(action.clip2D, action.wrapMode2D == ActionAnim.WrapMode2D.Loop, skeletonAnimation, action.layerInt);
						}
						else
						{
							SkeletonGraphic skeletonGraphic = action.runtimeAnim2D.GetComponent<SkeletonGraphic>();
							if (skeletonGraphic)
							{
								SetAnimation(action.clip2D, action.wrapMode2D == ActionAnim.WrapMode2D.Loop, skeletonGraphic, action.layerInt);
							}
							else
							{
								return 0f;
							}
						}

						if (action.wrapMode2D != ActionAnim.WrapMode2D.Loop && action.willWait)
						{
							CompletionListener completionListener = action.runtimeAnim2D.GetComponent<CompletionListener>();
							if (completionListener == null)
							{
								completionListener = action.runtimeAnim2D.gameObject.AddComponent<CompletionListener>();
							}

							completionListener.SetWaitingAnimation(action.clip2D);
							return action.defaultPauseTime;
						}
					}
					else if (action.method == AnimMethod.BlendShape)
					{
						ACDebug.LogWarning("BlendShapes not available for 2D animation.");
						return 0f;
					}
				}
			}
			else
			{
				CompletionListener completionListener = action.runtimeAnim2D.GetComponent<CompletionListener>();
				if (!completionListener.IsWaiting())
				{
					action.isRunning = false;
					return 0f;
				}

				return action.defaultPauseTime;
			}

			return 0f;
		}


		public override void ActionAnimSkip(ActionAnim action)
		{
			if (action.runtimeAnim2D != null && !string.IsNullOrEmpty(action.clip2D) && action.method == AnimMethod.PlayCustom)
			{
				SkeletonAnimation skeletonAnimation = action.runtimeAnim2D.GetComponent<SkeletonAnimation>();
				if (skeletonAnimation)
				{
					SetAnimation(action.clip2D, action.wrapMode2D == ActionAnim.WrapMode2D.Loop, skeletonAnimation, action.layerInt);
				}
				else
				{
					SkeletonGraphic skeletonGraphic = action.runtimeAnim2D.GetComponent<SkeletonGraphic>();
					if (skeletonGraphic)
					{
						SetAnimation(action.clip2D, action.wrapMode2D == ActionAnim.WrapMode2D.Loop, skeletonGraphic, action.layerInt);
					}
				}
			}
		}


		private void SetAnimation(string clip, bool doLoop, SkeletonAnimation skeletonAnimation, int trackIndex)
		{
			if (string.IsNullOrEmpty(clip) || skeletonAnimation == null)
			{
				return;
			}

			TrackEntry trackEntry = skeletonAnimation.state.GetCurrent(trackIndex);
			if (trackEntry != null)
			{
				if (trackEntry.Animation.Name == clip && trackEntry.Loop == doLoop)
				{
					return;
				}
			}

			skeletonAnimation.state.SetAnimation(trackIndex, clip, doLoop);
		}


		private void SetAnimation(string clip, bool doLoop, SkeletonGraphic skeletonGraphic, int trackIndex)
		{
			if (string.IsNullOrEmpty(clip) || skeletonGraphic == null)
			{
				return;
			}

			TrackEntry trackEntry = skeletonGraphic.AnimationState.GetCurrent(trackIndex);
			if (trackEntry != null)
			{
				if (trackEntry.Animation.Name == clip && trackEntry.Loop == doLoop)
				{
					return;
				}
			}

			skeletonGraphic.AnimationState.SetAnimation(trackIndex, clip, doLoop);
		}


		public override void ActionCharRenderGUI(ActionCharRender action)
		{
#if UNITY_EDITOR
			EditorGUILayout.Space();
			action.renderLock_scale = (RenderLock)EditorGUILayout.EnumPopup("Sprite scale:", action.renderLock_scale);
			if (action.renderLock_scale == RenderLock.Set)
			{
				action.scale = EditorGUILayout.IntField("New scale (%):", action.scale);
			}

			EditorGUILayout.Space();
			action.renderLock_direction = (RenderLock)EditorGUILayout.EnumPopup("Sprite direction:", action.renderLock_direction);
			if (action.renderLock_direction == RenderLock.Set)
			{
				action.direction = (CharDirection)EditorGUILayout.EnumPopup("New direction:", action.direction);
			}

			EditorGUILayout.Space();
			action.renderLock_sortingMap = (RenderLock)EditorGUILayout.EnumPopup("Sorting Map:", action.renderLock_sortingMap);
			if (action.renderLock_sortingMap == RenderLock.Set)
			{
				action.sortingMap = (SortingMap)EditorGUILayout.ObjectField("New Sorting Map:", action.sortingMap, typeof(SortingMap), true);
			}

			if (GUI.changed)
			{
				EditorUtility.SetDirty(action);
			}

#endif
		}


		public override float ActionCharRenderRun(ActionCharRender action)
		{
			if (action.renderLock_scale == RenderLock.Set)
			{
				character.lockScale = true;
				character.spriteScale = (float)action.scale / 100f;
			}
			else if (action.renderLock_scale == RenderLock.Release)
			{
				character.lockScale = false;
			}

			if (action.renderLock_direction == RenderLock.Set)
			{
				character.SetSpriteDirection(action.direction);
			}
			else if (action.renderLock_direction == RenderLock.Release)
			{
				character.lockDirection = false;
			}

			if (action.renderLock_sortingMap != RenderLock.NoChange && character.GetComponentInChildren<FollowSortingMap>())
			{
				FollowSortingMap[] followSortingMaps = character.GetComponentsInChildren<FollowSortingMap>();
				SortingMap sortingMap = (action.renderLock_sortingMap == RenderLock.Set) ? action.sortingMap : KickStarter.sceneSettings.sortingMap;

				foreach (FollowSortingMap followSortingMap in followSortingMaps)
				{
					followSortingMap.SetSortingMap(sortingMap);
				}
			}

			return 0f;
		}


		public override void PlayIdle()
		{
			PlayStandardAnim(character.idleAnimSprite);
			PlaySeparateHead();
		}


		public override void PlayWalk()
		{
			PlayStandardAnim(character.walkAnimSprite);
			PlaySeparateHead();
		}


		public override void PlayRun()
		{
			if (!string.IsNullOrEmpty(character.runAnimSprite))
			{
				PlayStandardAnim(character.runAnimSprite);
			}
			else
			{
				PlayWalk();
			}
			PlaySeparateHead();
		}


		public override void PlayTalk()
		{
			if (!string.IsNullOrEmpty(character.talkAnimSprite))
			{
				PlayStandardAnim(character.talkAnimSprite);
				PlaySeparateHead();
			}
			else
			{
				PlayIdle();
			}
		}


		private void PlaySeparateHead()
		{
			if (character.separateTalkingLayer)
			{
				if (character.isTalking)
				{
					PlayHeadAnim(character.talkAnimSprite);
				}
				else
				{
					PlayHeadAnim(character.idleAnimSprite);
				}
			}
		}


		private void PlayStandardAnim(string clip)
		{
			if (character != null)
			{
				SetDirection(character.GetSpriteDirection());

				if (!string.IsNullOrEmpty(clip))
				{
					PlayCharAnim(clip, 0);
				}
			}
		}


		private void PlayHeadAnim(string clip)
		{
			if (character && !string.IsNullOrEmpty(clip))
			{
				PlayCharAnim(clip, character.headLayer);
			}
		}


		private void PlayCharAnim(string clip, int trackIndex, bool doLoop = true)
		{
			if (DirectionMapper != null)
			{
				DirectionMapper.PlayClip(clip, trackIndex, doLoop);
			}
		}


		private void SetDirection(string directionSuffix)
		{
			DirectionMapper.SetCurrentDirection(directionSuffix);
		}


		private DirectionMapper directionMapper;
		private DirectionMapper DirectionMapper
		{
			get
			{
				if (directionMapper == null && character != null)
				{
					directionMapper = character.GetComponent<DirectionMapper>();
				}
				return directionMapper;
			}
		}

	}

}
