using KartverketRegister.Utils;
using System.Collections.Generic;
using Xunit;

namespace KartverketRegister.Tests.Utils
{
    public class HtmlEncodingExtensionsTests
    {
        private class SimpleModel
        {
            public string Name { get; set; }
            public string Description { get; set; }
        }

        private class NestedModel
        {
            public string Title { get; set; }
            public SimpleModel Child { get; set; }
        }

        private class CollectionModel
        {
            public List<SimpleModel> Items { get; set; }
        }

        [Fact]
        public void HtmlEncodeStrings_EncodesSimpleObject()
        {
            var obj = new SimpleModel
            {
                Name = "<Hello>",
                Description = "Test & More"
            };

            obj.HtmlEncodeStrings();

            Assert.Equal("&lt;Hello&gt;", obj.Name);
            Assert.Equal("Test &amp; More", obj.Description);
        }

        [Fact]
        public void HtmlEncodeStrings_EncodesNestedObject()
        {
            var obj = new NestedModel
            {
                Title = "Top <Tag>",
                Child = new SimpleModel
                {
                    Name = "Child & Name",
                    Description = null
                }
            };

            obj.HtmlEncodeStrings();

            Assert.Equal("Top &lt;Tag&gt;", obj.Title);
            Assert.Equal("Child &amp; Name", obj.Child.Name);
            Assert.Null(obj.Child.Description); // Null should remain null
        }

        
        public void HtmlEncodeStrings_NullObject_ReturnsNull()
        {
            SimpleModel obj = null;
            var result = obj.HtmlEncodeStrings();
            Assert.Null(result);
        }
    }
}
