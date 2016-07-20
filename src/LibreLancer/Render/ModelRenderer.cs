﻿/* The contents of this file are subject to the Mozilla Public License
 * Version 1.1 (the "License"); you may not use this file except in
 * compliance with the License. You may obtain a copy of the License at
 * http://www.mozilla.org/MPL/
 * 
 * Software distributed under the License is distributed on an "AS IS"
 * basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. See the
 * License for the specific language governing rights and limitations
 * under the License.
 * 
 * 
 * The Initial Developer of the Original Code is Callum McGing (mailto:callum.mcging@gmail.com).
 * Portions created by the Initial Developer are Copyright (C) 2013-2016
 * the Initial Developer. All Rights Reserved.
 */
using System;
using LibreLancer.Utf.Cmp;
using LibreLancer.Utf.Mat;
using LibreLancer.GameData;
namespace LibreLancer
{
	public class ModelRenderer
	{
		ICamera camera;
		public Matrix4 World { get; private set; }
		public SystemObject SpaceObject { get; private set; }
		public ModelFile Model { get; private set; }
		public CmpFile Cmp { get; private set; }
		public SphFile Sph { get; private set; }
		public NebulaRenderer Nebula;
		float radiusAtmosphere;

		public ModelRenderer (ICamera camera, Matrix4 world, SystemObject spaceObject,ResourceManager cache, NebulaRenderer nebula)
		{
			Nebula = nebula;
			World = world * Matrix4.CreateTranslation(spaceObject.Position);
			if (spaceObject.Rotation != null)
				World = spaceObject.Rotation.Value * World;
			SpaceObject = spaceObject;
			this.camera = camera;
			IDrawable archetype = spaceObject.Archetype.Drawable;
			if (archetype is ModelFile) {
				Model = archetype as ModelFile;
				if (Model != null && Model.Levels.ContainsKey (0)) {
					Model.Initialize (cache);
				}
			} else if (archetype is CmpFile) {
				Cmp = archetype as CmpFile;
				Cmp.Initialize (cache);
			} else if (archetype is SphFile) {
				Sph = archetype as SphFile;
				Sph.Initialize (cache);
				if (Sph.SideMaterials.Length > 6)
					radiusAtmosphere = Sph.Radius * Sph.SideMaterials[6].Scale;
				else
					radiusAtmosphere = Sph.Radius;
			}
		}

		public void Update(TimeSpan elapsed)
		{
			if (Model != null)
				Model.Update (camera, elapsed);
			else if (Cmp != null)
				Cmp.Update (camera, elapsed);
			else if (Sph != null)
				Sph.Update (camera, elapsed);
		}

		public bool LightInSphere(ref RenderLight lt)
		{
			if (Sph == null)
				return false;
			var bsphere = new BoundingSphere(
				SpaceObject.Position,
				radiusAtmosphere
			);
			return bsphere.Contains(lt.Position) == ContainmentType.Contains;
		}


		public void Draw(CommandBuffer buffer, Lighting lights, NebulaRenderer nr)
		{
			if (Nebula != null && nr != Nebula)
				return;
			if (Model != null) {
				if (Model.Levels.ContainsKey (0)) {
					var center = VectorMath.Transform(Model.Levels[0].Center, World);
					var bsphere = new BoundingSphere(
						center,
						Model.Levels[0].Radius
					);
					if (camera.Frustum.Intersects(bsphere)) {
						var lighting = RenderHelpers.ApplyLights(lights, center, Model.Levels[0].Radius, nr);
						if (!lighting.FogEnabled || VectorMath.Distance(camera.Position, center) <= Model.Levels[0].Radius + lighting.FogRange.Y)
							Model.DrawBuffer(buffer, World, lighting);
					}
				}
			} else if (Cmp != null) {
				foreach (Part p in Cmp.Parts.Values)
				{
					var model = p.Model;
					Matrix4 w = World;
					if (p.Construct != null)
						w = p.Construct.Transform * World;
					if (model.Levels.ContainsKey(0))
					{

						var center = VectorMath.Transform(model.Levels[0].Center, w);
						var bsphere = new BoundingSphere(
							center,
							model.Levels[0].Radius
						);
						if (camera.Frustum.Intersects(bsphere))
						{
							var lighting = RenderHelpers.ApplyLights(lights, center, model.Levels[0].Radius, nr);
							if (!lighting.FogEnabled || VectorMath.Distance(camera.Position, center) <= model.Levels[0].Radius + lighting.FogRange.Y)
								model.DrawBuffer(buffer, w, lighting);
						}
					}
				}
			} else if (Sph != null) {
				var bsphere = new BoundingSphere(
					SpaceObject.Position,
					radiusAtmosphere);
				if (camera.Frustum.Intersects(bsphere))
				{
					var l = RenderHelpers.ApplyLights(lights, SpaceObject.Position, Sph.Radius, nr);
					if(!l.FogEnabled || VectorMath.Distance(camera.Position, SpaceObject.Position) <= Sph.Radius + l.FogRange.Y)
						Sph.DrawBuffer(buffer, World, l);
				}
			}
		}
	}
}

