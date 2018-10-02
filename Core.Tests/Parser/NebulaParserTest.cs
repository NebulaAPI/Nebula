using System.Linq;
using Nebula.Parser;
using NUnit.Framework;

namespace Core.Tests.Parser
{
    [TestFixture]
    public class NebulaParserTest
    {
        private const string SIMPLE_ENTITY = @"
        entity Device {
            name: string
        }
        ";

        private const string COMPLEX_ENTITIES = @"
        entity ComplexDevice {
            name: string,
            id: integer,
            active: boolean,
            history: array[ComplexDeviceHistory]
        }

        entity ComplexDeviceHistory {
            id: integer,
            deviceId: integer
        }
        ";
        
        private const string SIMPLE_API = @"
        api SimpleApi {
            config {
                host = ""http://somehost.com"",
                prefix = ""/api"",
                authMethod = OAuthToken
            }

            func getDevices() << ""/devices"" -> array[Device]
        }
        ";
        
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void ComplexEntityTest()
        {
            var stream = new InputStream(COMPLEX_ENTITIES);
            var tokenizer = new Tokenizer(stream);
            var parser = new NebulaParser(tokenizer);
            var result = parser.Parse("sampleModule");

            var entities = result.SearchByType<EntityNode>();
            Assert.AreEqual(2, entities.Count);
            var complexDeviceNode = entities.FirstOrDefault(e => e.Name == "ComplexDevice");
            Assert.NotNull(complexDeviceNode);
            Assert.AreEqual(4, complexDeviceNode.Fields.Count);
            var complexDeviceHistoryNode = entities.FirstOrDefault(e => e.Name == "ComplexDeviceHistory");
            Assert.NotNull(complexDeviceHistoryNode);
            Assert.AreEqual(2, complexDeviceHistoryNode.Fields.Count);
        }

        [Test]
        public void SimpleApiTest()
        {
            var stream = new InputStream(SIMPLE_API);
            var tokenizer = new Tokenizer(stream);
            var parser = new NebulaParser(tokenizer);
            var result = parser.Parse("sampleModule");

            var apiNode = result.SearchByType<ApiNode>().FirstOrDefault();
            Assert.NotNull(apiNode);
            Assert.AreEqual("SimpleApi", apiNode.Name);

            var config = apiNode.SearchByType<ConfigNode>().FirstOrDefault();
            Assert.NotNull(config);
            var configNodes = config.Nodes;
            Assert.AreEqual(3, configNodes.Count);

            var functions = apiNode.SearchByType<FunctionNode>();
            Assert.AreEqual(1, functions.Count);
            
            var func = functions.FirstOrDefault();
            Assert.NotNull(func);
            Assert.AreEqual("getDevices", func.Name);
            Assert.AreEqual(TokenType.GetFunction, func.Method);
            Assert.AreEqual("/devices", func.Url);
            Assert.AreEqual("array", func.ReturnType.Name);
            Assert.IsTrue(func.ReturnType.Generic);
            Assert.AreEqual("Device", func.ReturnType.GenericType);
        }

        [Test]
        public void SimpleEntityTest()
        {
            var stream = new InputStream(SIMPLE_ENTITY);
            var tokenizer = new Tokenizer(stream);
            var parser = new NebulaParser(tokenizer);
            var result = parser.Parse("sampleModule");

            Assert.IsAssignableFrom(typeof(ModuleNode), result);
            Assert.AreEqual("sampleModule", result.Name);

            var entityNodes = result.SearchByType<EntityNode>();
            Assert.AreEqual(1, entityNodes.Count);
            Assert.IsAssignableFrom(typeof(EntityNode), entityNodes.FirstOrDefault());

            var deviceNode = entityNodes.FirstOrDefault();
            Assert.NotNull(deviceNode);
        }

        [Test]
        public void EntityFieldTest()
        {
            var stream = new InputStream(SIMPLE_ENTITY);
            var tokenizer = new Tokenizer(stream);
            var parser = new NebulaParser(tokenizer);
            var result = parser.Parse("sampleModule");

            var fields = result.SearchByType<ArgumentNode>();
            Assert.AreEqual(1, fields.Count);
        }
    }
}