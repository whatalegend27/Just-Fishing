// --- Decorator Pattern ---

public interface IStatCalculator
{
    int Calculate(int currentValue);
}

public class BaseStatCalculator : IStatCalculator
{
    public int Calculate(int currentValue) => currentValue;
}

public abstract class StatCalculatorDecorator : IStatCalculator
{
    protected IStatCalculator inner;
    public StatCalculatorDecorator(IStatCalculator inner) { this.inner = inner; }
    public abstract int Calculate(int currentValue);
}

public class StealRiskDecorator : StatCalculatorDecorator
{
    public StealRiskDecorator(IStatCalculator inner) : base(inner) {}
    public override int Calculate(int currentValue) => inner.Calculate(currentValue) + 5;
}

public class NightFishRiskDecorator : StatCalculatorDecorator
{
    public NightFishRiskDecorator(IStatCalculator inner) : base(inner) {}
    public override int Calculate(int currentValue) => inner.Calculate(currentValue) + 10;
}

public class BlackMarketRiskDecorator : StatCalculatorDecorator
{
    public BlackMarketRiskDecorator(IStatCalculator inner) : base(inner) {}
    public override int Calculate(int currentValue) => inner.Calculate(currentValue) + 5;
}

public class EatHungerDecorator : StatCalculatorDecorator
{
    public EatHungerDecorator(IStatCalculator inner) : base(inner) {}
    public override int Calculate(int currentValue) => inner.Calculate(currentValue) + 20;
}

public class TimeHungerDecorator : StatCalculatorDecorator
{
    public TimeHungerDecorator(IStatCalculator inner) : base(inner) {}
    public override int Calculate(int currentValue) => inner.Calculate(currentValue) - 5;
}

public class RestExhaustionDecorator : StatCalculatorDecorator
{
    public RestExhaustionDecorator(IStatCalculator inner) : base(inner) {}
    public override int Calculate(int currentValue) => inner.Calculate(currentValue) + 20;
}

public class FishExhaustionDecorator : StatCalculatorDecorator
{
    public FishExhaustionDecorator(IStatCalculator inner) : base(inner) {}
    public override int Calculate(int currentValue) => inner.Calculate(currentValue) - 5;
}

public class StealExhaustionDecorator : StatCalculatorDecorator
{
    public StealExhaustionDecorator(IStatCalculator inner) : base(inner) {}
    public override int Calculate(int currentValue) => inner.Calculate(currentValue) - 10;
}

public class HurtHealthDecorator : StatCalculatorDecorator
{
    public HurtHealthDecorator(IStatCalculator inner) : base(inner) {}
    public override int Calculate(int currentValue) => inner.Calculate(currentValue) - 10;
}
