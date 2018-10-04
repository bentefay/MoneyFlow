using System;
using LanguageExt;
using LanguageExt.UnitTesting;
using Newtonsoft.Json;
using Web.Utils.Serialization;
using Xunit;

namespace WebTests.Utils.Serialization
{
    public class OptionSerializerTests
    {
        [Fact]
        public void DeserializesSome()
        {
            var serialized = JsonConvert.DeserializeObject<TestClass>(
                @"{""intProperty"":1,""stringProperty"":""Hi!""}", 
                CreateSettings());
            
            serialized.IntProperty.ShouldBeSome(x => Assert.Equal(1, x));
            serialized.StringProperty.ShouldBeSome(x => Assert.Equal("Hi!", x));
        }
        
        [Fact]
        public void DeserializesNullToNone()
        {
            var serialized = JsonConvert.DeserializeObject<TestClass>(
                @"{""intProperty"":null,""stringProperty"":null}", 
                CreateSettings());
            
            serialized.IntProperty.ShouldBeNone();
            serialized.StringProperty.ShouldBeNone();
        }
        
        // Unfortunately, the CamelCasePropertyNamesContractResolver and DefaultContractResolver
        // have static state that causes DeserializesNullToNone and DeserializesEmptyToNone to conflict.
        // Hence only one can be run at a time.
//        [Fact]
//        public void DeserializesEmptyToNone()
//        {
//            var serialized = JsonConvert.DeserializeObject<TestClass>(
//                @"{}", 
//                CreateSettings());
//            
//            serialized.IntProperty.ShouldBeNone();
//            serialized.StringProperty.ShouldBeNone();
//        }
        
        [Fact]
        public void SerializesSome()
        {
            var serialized = JsonConvert.SerializeObject(
                new TestClass("Hi!", 1), 
                CreateSettings());
            
            Assert.Equal(@"{""intProperty"":1,""stringProperty"":""Hi!""}", serialized);
        }

        [Fact]
        public void GivenIgnoreNullsThenNoneSerializesToEmpty()
        {
            var serialized = JsonConvert.SerializeObject(
                new TestClass(Prelude.None, Prelude.None), 
                CreateSettings(s => s.NullValueHandling = NullValueHandling.Ignore));
            
            Assert.Equal(@"{}", serialized);
        }
        
        [Fact]
        public void GivenIncludeNullsThenNoneSerializesToNull()
        {
            var serialized = JsonConvert.SerializeObject(
                new TestClass(Prelude.None, Prelude.None), 
                CreateSettings(s => s.NullValueHandling = NullValueHandling.Include));
            
            Assert.Equal(@"{""intProperty"":null,""stringProperty"":null}", serialized);
        }

        private static JsonSerializerSettings CreateSettings(Action<JsonSerializerSettings> configure = null)
        {
            var settings = new JsonSerializerSettings();
            configure?.Invoke(settings);
            settings.Converters.Add(new OptionJsonConverter());
            settings.ContractResolver = new OptionContractResolver(settings.NullValueHandling);
            return settings;
        }
        
        public class TestClass
        {
            public TestClass(Option<string> stringProperty, Option<int> intProperty)
            {
                StringProperty = stringProperty;
                IntProperty = intProperty;
            }

            public Option<int> IntProperty { get; }
            public Option<string> StringProperty { get; }
        }
    }    
}