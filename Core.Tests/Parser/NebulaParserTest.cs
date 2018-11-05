using System.Linq;
using Nebula.Core.Parser;
using Nebula.SDK.Objects;
using NUnit.Framework;

namespace Core.Tests.Parser
{
    [TestFixture]
    public class NebulaParserTest
    {
        private const string SimpleEntity = @"
        entity Device {
            name: string
        }
        ";

        private const string ComplexEntities = @"
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
        
        private const string SimpleApi = @"
        api SimpleApi {
            config {
                host = ""http://somehost.com"",
                prefix = ""/api"",
                authMethod = OAuthToken
            }

            func getDevices() << ""/devices"" -> array[Device] {
                description = ""Get all the devices"",
                return = ""Array of Device objects""
            }
        }
        ";

        private const string ComplexApi = @"
        api ComplexApi {
            config {
                host =""http://somehost.com"",
                prefix = ""/api"",
                authMethod = OAuthToken
            }

            func getDevices() << ""/devices"" -> array[Device] {
                description = ""Get all the devices"",
                return = ""Array of Device objects""
            }

            func getDeviceById(id: integer) << ""/device/{id}"" -> Device {
                description = ""Get a single device by id"",
                return = ""Device object"",
                id = ""The ID of the device to retrieve""
            }

            func createDevice(device: Device) >> ""/device/new"" -> Device {
                description = ""Create a new device"",
                return = ""The Device that was created"",
                device = ""The Device data to use""
            }
        }
        ";
        
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void ComplexApiTest()
        {
            var stream = new InputStream(ComplexApi);
            var tokenizer = new Tokenizer(stream);
            var parser = new NebulaParser(tokenizer);
            var result = parser.Parse("sampleModule");

            var api = result.SearchByType<ApiNode>().FirstOrDefault();
            Assert.NotNull(api);
            var funcs = result.SearchByType<FunctionNode>();
            Assert.AreEqual(3, funcs.Count);
            var getDevicesFunc = funcs.FirstOrDefault(f => f.Name == "getDevices");
            Assert.NotNull(getDevicesFunc);
            Assert.AreEqual("/devices", getDevicesFunc.Url);
            Assert.AreEqual(TokenType.GetFunction, getDevicesFunc.Method);
        }

        [Test]
        public void ComplexEntityTest()
        {
            var stream = new InputStream(ComplexEntities);
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
            var stream = new InputStream(SimpleApi);
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
            var stream = new InputStream(SimpleEntity);
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
            var stream = new InputStream(SimpleEntity);
            var tokenizer = new Tokenizer(stream);
            var parser = new NebulaParser(tokenizer);
            var result = parser.Parse("sampleModule");

            var fields = result.SearchByType<ArgumentNode>();
            Assert.AreEqual(1, fields.Count);
        }
    }
}