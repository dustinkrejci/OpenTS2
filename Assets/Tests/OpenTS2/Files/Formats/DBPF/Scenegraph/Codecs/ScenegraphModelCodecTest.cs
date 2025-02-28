﻿
    using NUnit.Framework;
    using OpenTS2.Common;
    using OpenTS2.Content;
    using OpenTS2.Content.DBPF.Scenegraph;
    using OpenTS2.Files.Formats.DBPF;

    public class ScenegraphModelCodecTest
    {
        [SetUp]
        public void SetUp()
        {
            TestMain.Initialize();
            ContentProvider.Get().AddPackage("TestAssets/Scenegraph/teapot_model.package");
        }
        
        [Test]
        public void TestLoadsTeapotModelWithCorrectVerticesAndFaceCount()
        {
            var modelAsset =
                ContentProvider.Get().GetAsset<ScenegraphModelAsset>(new ResourceKey("teapot_tslocator_gmdc", 0x1C0532FA,
                    TypeIDs.SCENEGRAPH_GMDC));

            // There are technically ways to compare meshes properly but just use the face/vertex count here as a
            // close approximation :)
            Assert.That(modelAsset.StaticBoundMesh.vertexCount, Is.EqualTo(3241));
            Assert.That(modelAsset.StaticBoundMesh.triangles.Length / 3, Is.EqualTo(6320));
        }
    }
