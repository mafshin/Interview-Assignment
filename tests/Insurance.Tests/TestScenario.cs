namespace Insurance.Tests
{
    public class TestScenario<T>
    {
        public TestScenario(T testScenarioModel, string testName)
        {
            Model = testScenarioModel;
            TestName = testName;
        }

        public T Model { get; }

        public string TestName { get; }

        public override string ToString() => TestName;

        public object[] ToObjectArray() => new object[] {this};
    }
}