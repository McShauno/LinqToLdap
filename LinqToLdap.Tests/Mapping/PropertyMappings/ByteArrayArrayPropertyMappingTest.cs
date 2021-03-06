﻿using System;
using System.DirectoryServices.Protocols;
using LinqToLdap.Mapping.PropertyMappings;
using LinqToLdap.Tests.TestSupport.ExtensionMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpTestsEx;

namespace LinqToLdap.Tests.Mapping.PropertyMappings
{
    [TestClass]
    public class ByteArrayArrayPropertyMappingTest
    {
        private PropertyMappingArguments<ByteArrayArrayPropertyMappingTest> _mappingArguments;

        [TestInitialize]
        public void SetUp()
        {
            _mappingArguments = new PropertyMappingArguments<ByteArrayArrayPropertyMappingTest>
            {
                AttributeName = "att",
                PropertyName = "name",
                PropertyType = typeof(object)
            };
        }

        [TestMethod]
        public void FormatValueToFilter_ByteArrayArray_ThrowsNotSupportedException()
        {
            //prepare
            var bytes = new[] { new byte[] { 1, 2 }, new byte[] { 3, 4 } };
            var propertyMapping = new ByteArrayArrayPropertyMapping<ByteArrayArrayPropertyMappingTest>(_mappingArguments);

            //act
            Executing.This(() => propertyMapping.FormatValueToFilter(bytes)).Should()
                .Throw<NotSupportedException>();
        }

        [TestMethod]
        public void FormatValueToFilter_ByteArray_ReturnsStringOctet()
        {
            //prepare
            var bytes = new byte[] {1, 2, 3, 4};
            var propertyMapping = new ByteArrayArrayPropertyMapping<ByteArrayArrayPropertyMappingTest>(_mappingArguments);

            //act
            var value = propertyMapping.FormatValueToFilter(bytes);

            //assert
            value.Should().Be.EqualTo(bytes.ToStringOctet());
        }

        [TestMethod]
        public void FormatValueFromDirectory_ByteArrayArray_ReturnsByteArrayArray()
        {
            //prepare
            var bytes = new[] { new byte[] { 1, 2 }, new byte[] { 3, 4 } };
            var propertyMapping = new ByteArrayArrayPropertyMapping<ByteArrayArrayPropertyMappingTest>(_mappingArguments);

            //act
            var value = propertyMapping.FormatValueFromDirectory(new DirectoryAttribute("name", bytes), "dn");

            //assert
            value.As<byte[][]>().Should().Have.SameSequenceAs(bytes);
        }

        [TestMethod]
        public void FormatValueFromDirectory_Null_ReturnsNull()
        {
            //prepare
            _mappingArguments.PropertyType = typeof(byte[][]);
            var propertyMapping = new ByteArrayArrayPropertyMapping<ByteArrayArrayPropertyMappingTest>(_mappingArguments);

            //act
            var value = propertyMapping.FormatValueFromDirectory(null, "dn");

            //assert
            value.Should().Be.Null();
        }

        [TestMethod]
        public void FormatValueFromDirectory_SingleByteArray_ReturnsAsByteArrayArray()
        {
            //prepare
            var bytes = new byte[] { 1, 2, 3, 4 };
            _mappingArguments.PropertyType = typeof(byte[][]);
            var propertyMapping = new ByteArrayArrayPropertyMapping<ByteArrayArrayPropertyMappingTest>(_mappingArguments);

            //act
            var value = propertyMapping.FormatValueFromDirectory(new DirectoryAttribute("name", bytes), "dn");

            //assert
            value.As<byte[][]>().Should().Contain(bytes);
        }

        [TestMethod]
        public void IsEqual_SameLengthDifferentArrays_ReturnsFalse()
        {
            //prepare
            _mappingArguments.Getter = t => new[] { new byte[] { 1, 2, 3 }, new byte[] { 4, 5, 6 } };
            var propertyMapping = new ByteArrayArrayPropertyMapping<ByteArrayArrayPropertyMappingTest>(_mappingArguments);
            DirectoryAttributeModification modification;
            //act
            var value = propertyMapping.IsEqual(this, new[] { new byte[] { 1, 2, 3 }, new byte[] { 4, 5, 7 } }, out modification);

            //assert
            value.Should().Be.False();
            modification.Should().Not.Be.Null();
        }

        [TestMethod]
        public void IsEqual_DifferentLengths_ReturnsFalse()
        {
            //prepare
            _mappingArguments.Getter = t => new[] { new byte[] { 1, 2, 3 } };
            var propertyMapping = new ByteArrayArrayPropertyMapping<ByteArrayArrayPropertyMappingTest>(_mappingArguments);
            DirectoryAttributeModification modification;
            //act
            var value = propertyMapping.IsEqual(this, new[] { new byte[] { 1, 2, 3 }, new byte[] { 4, 5, 7 } }, out modification);

            //assert
            value.Should().Be.False();
            modification.Should().Not.Be.Null();
        }

        [TestMethod]
        public void IsEqual_OneNull_ReturnsFalse()
        {
            //prepare
            _mappingArguments.Getter = t => null;
            var propertyMapping = new ByteArrayArrayPropertyMapping<ByteArrayArrayPropertyMappingTest>(_mappingArguments);
            DirectoryAttributeModification modification;
            //act
            var value = propertyMapping.IsEqual(this, new[] { new byte[] { 1, 2, 3 }, new byte[] { 4, 5, 7 } }, out modification);

            //assert
            value.Should().Be.False();
            modification.Should().Not.Be.Null();
        }

        [TestMethod]
        public void IsEqual_SameArrays_ReturnsFalse()
        {
            //prepare
            _mappingArguments.Getter = t => new[] { new byte[] { 1, 2, 3 }, new byte[] { 4, 5, 6 } };
            var propertyMapping = new ByteArrayArrayPropertyMapping<ByteArrayArrayPropertyMappingTest>(_mappingArguments);
            DirectoryAttributeModification modification;
            //act
            var value = propertyMapping.IsEqual(this, new[] { new byte[] { 1, 2, 3 }, new byte[] { 4, 5, 6 } }, out modification);

            //assert
            value.Should().Be.True();
            modification.Should().Be.Null();
        }
    }
}
