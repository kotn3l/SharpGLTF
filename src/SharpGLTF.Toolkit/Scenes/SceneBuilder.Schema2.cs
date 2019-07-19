﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;

namespace SharpGLTF.Scenes
{
    using Schema2;

    using MESHBUILDER = Geometry.IMeshBuilder<Materials.MaterialBuilder>;

    class Schema2SceneBuilder
    {
        #region data

        private readonly Dictionary<MESHBUILDER, Mesh> _Meshes = new Dictionary<MESHBUILDER, Mesh>();

        private readonly Dictionary<NodeBuilder, Node> _Nodes = new Dictionary<NodeBuilder, Node>();

        #endregion

        #region API

        public Mesh GetMesh(MESHBUILDER key) { return key == null ? null : _Meshes.TryGetValue(key, out Mesh val) ? val : null; }

        public Node GetNode(NodeBuilder key) { return key == null ? null : _Nodes.TryGetValue(key, out Node val) ? val : null; }

        public void AddScene(Scene dstScene, SceneBuilder srcScene)
        {
            // gather all MeshBuilder unique instances
            // and group them by their vertex attribute layout.

            var meshGroups = srcScene.Instances
                .Select(item => item.GetGeometryAsset())
                .Where(item => item != null)
                .Distinct()
                .ToList()
                .GroupBy(item => item.GetType());

            // create Schema2.Mesh collections for every gathered group.

            foreach (var meshGroup in meshGroups)
            {
                var meshArray = meshGroup.ToArray();

                var meshDst = dstScene.LogicalParent.CreateMeshes(meshArray);

                for (int i = 0; i < meshArray.Length; ++i)
                {
                    _Meshes[meshArray[i]] = meshDst[i];
                }
            }

            // gather all NodeBuilder unique armatures

            var armatures = srcScene.Instances
                .Select(item => item.GetArmatureAsset())
                .Where(item => item != null)
                .Select(item => item.Root)
                .Distinct()
                .ToList();

            // create Schema2.Node trees for every armature

            foreach (var armature in armatures)
            {
                CreateArmature(dstScene,  armature);
            }

            // process instances

            foreach (var inst in srcScene.Instances)
            {
                inst.Setup(dstScene, this);
            }
        }

        /// <summary>
        /// Recursively converts all the <see cref="NodeBuilder"/> instances into <see cref="Schema2.Node"/> instances.
        /// </summary>
        /// <param name="container">The target <see cref="Schema2.Scene"/> or <see cref="Schema2.Node"/>.</param>
        /// <param name="srcNode">The source <see cref="NodeBuilder"/> instance.</param>
        private void CreateArmature(IVisualNodeContainer container, NodeBuilder srcNode)
        {
            var dstNode = container.CreateNode(srcNode.Name);
            _Nodes[srcNode] = dstNode;

            if (srcNode.HasAnimations)
            {
                dstNode.LocalTransform = srcNode.LocalTransform;

                // Copies all the animations to the target node.
                if (srcNode.Scale != null) foreach (var t in srcNode.Scale.Tracks) dstNode.WithScaleAnimation(t.Key, t.Value);
                if (srcNode.Rotation != null) foreach (var t in srcNode.Rotation.Tracks) dstNode.WithRotationAnimation(t.Key, t.Value);
                if (srcNode.Translation != null) foreach (var t in srcNode.Translation.Tracks) dstNode.WithTranslationAnimation(t.Key, t.Value);
            }
            else
            {
                dstNode.LocalMatrix = srcNode.LocalMatrix;
            }

            foreach (var c in srcNode.Children) CreateArmature(dstNode, c);
        }

        #endregion
    }

    public partial class SceneBuilder
    {
        public ModelRoot ToSchema2()
        {
            var dstModel = ModelRoot.CreateModel();

            var dstScene = dstModel.UseScene(0);

            var context = new Schema2SceneBuilder();
            context.AddScene(dstScene, this);

            return dstModel;
        }
    }
}
