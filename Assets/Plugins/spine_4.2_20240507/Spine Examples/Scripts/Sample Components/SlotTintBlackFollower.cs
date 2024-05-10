/******************************************************************************
 * Spine Runtimes License Agreement
 * Last updated July 28, 2023. Replaces all prior versions.
 *
 * Copyright (c) 2013-2023, Esoteric Software LLC
 *
 * Integration of the Spine Runtimes into software or otherwise creating
 * derivative works of the Spine Runtimes is permitted under the terms and
 * conditions of Section 2 of the Spine Editor License Agreement:
 * http://esotericsoftware.com/spine-editor-license
 *
 * Otherwise, it is permitted to integrate the Spine Runtimes into software or
 * otherwise create derivative works of the Spine Runtimes (collectively,
 * "Products"), provided that each user of the Products must obtain their own
 * Spine Editor license and redistribution of the Products in any form must
 * include this license and copyright notice.
 *
 * THE SPINE RUNTIMES ARE PROVIDED BY ESOTERIC SOFTWARE LLC "AS IS" AND ANY
 * EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL ESOTERIC SOFTWARE LLC BE LIABLE FOR ANY
 * DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES,
 * BUSINESS INTERRUPTION, OR LOSS OF USE, DATA, OR PROFITS) HOWEVER CAUSED AND
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THE
 * SPINE RUNTIMES, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 *****************************************************************************/

using Spine.Unity;
using UnityEngine;

namespace Spine.Unity.Examples {

	/// <summary>
	/// Add this component to a Spine GameObject to apply a specific slot's Colors as MaterialProperties.
	/// This allows you to apply the two color tint to the whole skeleton and not require the overhead of an extra vertex stream on the mesh.
	/// </summary>
	public class SlotTintBlackFollower : MonoBehaviour {
		#region Inspector
		/// <summary>
		/// Serialized name of the slot loaded at runtime. Change the slot field instead of this if you want to change the followed slot at runtime.</summary>
		[SpineSlot]
		[SerializeField]
		protected string slotName;

		[SerializeField]
		protected string colorPropertyName = "_Color";
		[SerializeField]
		protected string blackPropertyName = "_Black";
		#endregion

		public Slot slot;
		MeshRenderer mr;
		MaterialPropertyBlock mb;
		int colorPropertyId, blackPropertyId;

		void Start () {
			Initialize(false);
		}

		public void Initialize (bool overwrite) {
			if (overwrite || mb == null) {
				mb = new MaterialPropertyBlock();
				mr = GetComponent<MeshRenderer>();
				slot = GetComponent<ISkeletonComponent>().Skeleton.FindSlot(slotName);

				colorPropertyId = Shader.PropertyToID(colorPropertyName);
				blackPropertyId = Shader.PropertyToID(blackPropertyName);
			}
		}

		public void Update () {
			Slot s = slot;
			if (s == null) return;

			mb.SetColor(colorPropertyId, s.GetColor());
			mb.SetColor(blackPropertyId, s.GetColorTintBlack());

			mr.SetPropertyBlock(mb);
		}

		void OnDisable () {
			mb.Clear();
			mr.SetPropertyBlock(mb);
		}
	}
}
