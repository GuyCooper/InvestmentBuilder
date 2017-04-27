using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using InvestmentBuilderCore;

namespace InvestmentBuilderMSTests
{
    [TestClass]
    public class NornaliseDataseTests
    {
        class IntClass
        {
            public IntClass(int i) { i_ = i; }
            public int i_;
            public override bool Equals(object obj)
            {
                return ((IntClass)obj).i_ == i_;
            }
            public override int GetHashCode()
            {
                return i_;
            }
        }

        private IList<IntClass> _ToIntClassRange(IEnumerable<int> range)
        {
            return range.Select(x => new IntClass(x)).ToList();
        }

        [TestMethod]
        public void WhenNormalisingRange2Window3()
        {
            var dataset = _ToIntClassRange(Enumerable.Range(1, 2));
            var match = _ToIntClassRange(new List<int>{ 1, 2});
            var result = DatasetNormaliser.NormaliseDataset<IntClass>(dataset, 3);
            Assert.IsTrue(result.SequenceEqual(match));
        }

        [TestMethod]
        public void WhenNormalisingRange3Window3()
        {
            var dataset = _ToIntClassRange(Enumerable.Range(1, 3));
            var match = _ToIntClassRange(new List<int> { 1, 2, 3 });
            var result = DatasetNormaliser.NormaliseDataset<IntClass>(dataset, 3);
            Assert.IsTrue(result.SequenceEqual(match));
        }

        [TestMethod]
        public void WhenNormalisingRange4Window3()
        {
            var dataset = _ToIntClassRange(Enumerable.Range(1, 4));
            var match = _ToIntClassRange(new List<int> { 1, 3, 4 });
            var result = DatasetNormaliser.NormaliseDataset<IntClass>(dataset, 3);
            Assert.IsTrue(result.SequenceEqual(match));
        }

        [TestMethod]
        public void WhenNormalisingRange5Window3()
        {
            var dataset = _ToIntClassRange(Enumerable.Range(1, 5));
            var match = _ToIntClassRange(new List<int> { 1, 3, 5 });
            var result = DatasetNormaliser.NormaliseDataset<IntClass>(dataset, 3);
            Assert.IsTrue(result.SequenceEqual(match));
        }

        [TestMethod]
        public void WhenNormalisingRange6Window3()
        {
            var dataset = _ToIntClassRange(Enumerable.Range(1, 6));
            var match = _ToIntClassRange(new List<int> { 1, 3, 6 });
            var result = DatasetNormaliser.NormaliseDataset<IntClass>(dataset, 3);
            Assert.IsTrue(result.SequenceEqual(match));
        }

        [TestMethod]
        public void WhenNormalisingRange7Window3()
        {
            var dataset = _ToIntClassRange(Enumerable.Range(1, 7));
            var match = _ToIntClassRange(new List<int> { 1, 4, 7 });
            var result = DatasetNormaliser.NormaliseDataset<IntClass>(dataset, 3);
            Assert.IsTrue(result.SequenceEqual(match));
        }

        [TestMethod]
        public void WhenNormalisingRange5Window5()
        {
            var dataset = _ToIntClassRange(Enumerable.Range(1, 5));
            var match = _ToIntClassRange(new List<int> { 1, 2, 3, 4, 5});
            var result = DatasetNormaliser.NormaliseDataset<IntClass>(dataset, 5);
            Assert.IsTrue(result.SequenceEqual(match));
        }

        [TestMethod]
        public void WhenNormalisingRange6Window5()
        {
            var dataset = _ToIntClassRange(Enumerable.Range(1, 6));
            var match = _ToIntClassRange(new List<int> { 1, 3, 4, 5, 6 });
            var result = DatasetNormaliser.NormaliseDataset<IntClass>(dataset, 5);
            Assert.IsTrue(result.SequenceEqual(match));
        }

        [TestMethod]
        public void WhenNormalisingRange7Window5()
        {
            var dataset = _ToIntClassRange(Enumerable.Range(1, 7));
            var match = _ToIntClassRange(new List<int> { 1, 3, 5, 6, 7 });
            var result = DatasetNormaliser.NormaliseDataset<IntClass>(dataset, 5);
            Assert.IsTrue(result.SequenceEqual(match));
        }

        [TestMethod]
        public void WhenNormalisingRange8Window5()
        {
            var dataset = _ToIntClassRange(Enumerable.Range(1, 8));
            var match = _ToIntClassRange(new List<int> { 1, 3, 5, 7, 8 });
            var result = DatasetNormaliser.NormaliseDataset<IntClass>(dataset, 5);
            Assert.IsTrue(result.SequenceEqual(match));
        }

        [TestMethod]
        public void WhenNormalisingRange9Window5()
        {
            var dataset = _ToIntClassRange(Enumerable.Range(1, 9));
            var match = _ToIntClassRange(new List<int> { 1, 3, 5, 7, 9 });
            var result = DatasetNormaliser.NormaliseDataset<IntClass>(dataset, 5);
            Assert.IsTrue(result.SequenceEqual(match));
        }

        [TestMethod]
        public void WhenNormalisingRange10Window5()
        {
            var dataset = _ToIntClassRange(Enumerable.Range(1, 10));
            var match = _ToIntClassRange(new List<int> { 1, 3, 5, 7, 10 });
            var result = DatasetNormaliser.NormaliseDataset<IntClass>(dataset, 5);
            Assert.IsTrue(result.SequenceEqual(match));
        }

        [TestMethod]
        public void WhenNormalisingRange11Window5()
        {
            var dataset = _ToIntClassRange(Enumerable.Range(1, 11));
            var match = _ToIntClassRange(new List<int> { 1, 4, 6, 8, 11 });
            var result = DatasetNormaliser.NormaliseDataset<IntClass>(dataset, 5);
            Assert.IsTrue(result.SequenceEqual(match));
        }

        [TestMethod]
        public void WhenNormalisingRange8Window8()
        {
            var dataset = _ToIntClassRange(Enumerable.Range(1, 8));
            var match = _ToIntClassRange(new List<int> { 1,2,3,4,5,6,7,8 });
            var result = DatasetNormaliser.NormaliseDataset<IntClass>(dataset, 8);
            Assert.IsTrue(result.SequenceEqual(match));
        }

        [TestMethod]
        public void WhenNormalisingRange9Window8()
        {
            var dataset = _ToIntClassRange(Enumerable.Range(1, 9));
            var match = _ToIntClassRange(new List<int> { 1, 3, 4, 5, 6, 7, 8, 9 });
            var result = DatasetNormaliser.NormaliseDataset<IntClass>(dataset, 8);
            Assert.IsTrue(result.SequenceEqual(match));
        }

        [TestMethod]
        public void WhenNormalisingRange19Window8()
        {
            var dataset = _ToIntClassRange(Enumerable.Range(1, 19));
            var match = _ToIntClassRange(new List<int> { 1, 5, 8, 10, 12, 14, 16, 19 });
            var result = DatasetNormaliser.NormaliseDataset<IntClass>(dataset, 8);
            Assert.IsTrue(result.SequenceEqual(match));
        }

        [TestMethod]
        public void WhenNormalisingRange9Window12()
        {
            var dataset = _ToIntClassRange(Enumerable.Range(1, 9));
            var match = _ToIntClassRange(new List<int> { 1,2,3,4,5,6,7,8,9 });
            var result = DatasetNormaliser.NormaliseDataset<IntClass>(dataset, 12);
            Assert.IsTrue(result.SequenceEqual(match));
        }

        [TestMethod]
        public void WhenNormalisingRange14Window12()
        {
            var dataset = _ToIntClassRange(Enumerable.Range(1, 14));
            var match = _ToIntClassRange(new List<int> { 1,3,5,6,7,8,9,10,11,12,13,14 });
            var result = DatasetNormaliser.NormaliseDataset<IntClass>(dataset, 12);
            Assert.IsTrue(result.SequenceEqual(match));
        }

        [TestMethod]
        public void WhenNormalisingRange30Window12()
        {
            var dataset = _ToIntClassRange(Enumerable.Range(1, 30));
            var match = _ToIntClassRange(new List<int> { 1,5,9,13,15,17,19,21,23,25,27,30 });
            var result = DatasetNormaliser.NormaliseDataset<IntClass>(dataset, 12);
            Assert.IsTrue(result.SequenceEqual(match));
        }

    }
}
