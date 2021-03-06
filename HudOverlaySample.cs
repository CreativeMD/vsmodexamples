﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace VSExampleMods
{
    /// <summary>
    /// Renders a progress bar hud in the top left corner of the screen
    /// </summary>
    public class HudOverlaySample : ModBase
    {
        ICoreClientAPI capi;
        WeirdProgressBarRenderer renderer;

        public override ModInfo GetModInfo()
        {
            return null;
        }

        public override bool ShouldLoad(EnumAppSide side)
        {
            return side == EnumAppSide.Client;
        }

        public override void StartClientSide(ICoreClientAPI api)
        {
            this.capi = api;
            renderer = new WeirdProgressBarRenderer(api);

            api.Event.RegisterRenderer(renderer, EnumRenderStage.Ortho);
        }
    }


    public class WeirdProgressBarRenderer : IRenderer
    {
        MeshRef whiteRectangleRef;
        MeshRef progressQuadRef;
        ICoreClientAPI capi;

        public double RenderOrder { get { return 0; } }

        public int RenderRange { get { return 10; } }

        public WeirdProgressBarRenderer(ICoreClientAPI api)
        {
            this.capi = api;

            // This will get a line loop with vertices inside [-1,-1] till [1,1]
            MeshData rectangle = LineMeshUtil.GetRectangle(ColorUtil.WhiteArgb);
            whiteRectangleRef = api.Render.UploadMesh(rectangle);

            // This will get a quad with vertices inside [-1,-1] till [1,1]
            progressQuadRef = api.Render.UploadMesh(QuadMeshUtil.GetQuad());
        }


        public void OnRenderFrame(float deltaTime, EnumRenderStage stage)
        {
            IShaderProgram curShader = capi.Render.CurrentActiveShader;

            Vec4f color = new Vec4f(1,1,1,1);
            
            // Render rectangle
            curShader.Uniform("rgbaIn", color);
            curShader.Uniform("extraGlow", 0);            
            curShader.Uniform("applyColor", 0);
            curShader.Uniform("tex2d", 0);
            curShader.Uniform("noTexture", 1f);

            capi.Render.GlPushMatrix();

            capi.Render.GlTranslate(10, 10, 50);
            capi.Render.GlScale(100, 20, 0);

            capi.Render.GlTranslate(0.5f, 0.5f, 0);
            capi.Render.GlScale(0.5f, 0.5f, 0);

            curShader.UniformMatrix("projectionMatrix", capi.Render.CurrentProjectionMatrix);
            curShader.UniformMatrix("modelViewMatrix", capi.Render.CurrentModelviewMatrix);

            capi.Render.RenderMesh(whiteRectangleRef);
            capi.Render.GlPopMatrix();


            // Render progress bar
            float width = (capi.World.ElapsedMilliseconds / 10f) % 100;

            capi.Render.GlPushMatrix();
            capi.Render.GlTranslate(10, 10, 50);
            capi.Render.GlScale(width, 20, 0);

            capi.Render.GlTranslate(0.5f, 0.5f, 0);
            capi.Render.GlScale(0.5f, 0.5f, 0);


            curShader.UniformMatrix("projectionMatrix", capi.Render.CurrentProjectionMatrix);
            curShader.UniformMatrix("modelViewMatrix", capi.Render.CurrentModelviewMatrix);

            capi.Render.RenderMesh(progressQuadRef);
            capi.Render.GlPopMatrix();

        }



        public void Dispose()
        {
            capi.Render.DeleteMesh(whiteRectangleRef);
            capi.Render.DeleteMesh(progressQuadRef);
        }


    }
}
