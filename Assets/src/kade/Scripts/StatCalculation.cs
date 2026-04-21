// --- Decorator Pattern ---
/*                        ┌───────────────────────────────┐
                          │        «interface»            │
                          │       IStatCalculator         │
                          ├───────────────────────────────┤
                          │ + calculate(currentValue):int │
                          └───────────────▲───────────────┘
                                          │
                        ┌─────────────────┴─────────────────┐
                        │ (implements)                      │ (implements)
            ┌───────────┴──────────┐         ┌──────────────┴──────────────────┐
            │  BaseStatCalculator  │         │   StatCalculatorDecorator       │
            │                      │         │         (abstract)              │
            ├──────────────────────┤         ├─────────────────────────────────┤
            │ + calculate(v): int  │         │ # mInner: IStatCalculator       │
            │   returns v          │         ├─────────────────────────────────┤
            └──────────────────────┘         │ + ctor(inner: IStatCalculator)  │ statcalculatordecorator
                                             │ + calculate(v): int  (abstract) │
                                             └──────────────┬──────────────────┘
                                                            │
                                                            │ wraps (mInner ◇)
                                                            │
                         ┌──────────────────────────────────┼──────────────────────────────────┐
                         │                                  │                                  │
              ─── RISK ──┴──────────────────── HUNGER ──────┴─────────────── HEALTH ───────────┴────
                         │                                  │                                  │
        ┌────────────────┼────────────────┐        ┌────────┴────────┐        ┌────────────────┼──────────────────┐
        ▼                ▼                ▼        ▼                 ▼        ▼                ▼                  ▼
┌───────────────┐ ┌────────────────┐ ┌──────────┐ ┌──────────────┐ ┌──────────────┐ ┌──────────────┐ ┌──────────────┐ ┌────────────────┐
│ StealRisk     │ │ NightFishRisk  │ │BlackMkt  │ │ EatHunger    │ │ TimeHunger   │ │ HealHealth   │ │ HurtHealth   │ │ TakeDamage     │
│ Decorator     │ │ Decorator      │ │Risk Dec. │ │ Decorator    │ │ Decorator    │ │ Decorator    │ │ Decorator    │ │ Decorator      │
├───────────────┤ ├────────────────┤ ├──────────┤ ├──────────────┤ ├──────────────┤ ├──────────────┤ ├──────────────┤ ├────────────────┤
│ inner + 5     │ │ inner + 10     │ │inner + 5 │ │ inner + 20   │ │ inner - 5    │ │ inner + 25   │ │ inner - 10   │ │ - mAmount: int │
│ (risk)        │ │ (risk)         │ │ (risk)   │ │ (hunger)     │ │ (hunger)     │ │ (health)     │ │ (health)     │ │ inner - mAmount│
└───────────────┘ └────────────────┘ └──────────┘ └──────────────┘ └──────────────┘ └──────────────┘ └──────────────┘ └────────────────┘

Legend:
  ▲  realization / inheritance (implements or extends)
  ◇  aggregation — decorator holds a reference to an inner IStatCalculator
  #  protected    + public
  */
/* Interface for all stat calculators.
   Each implementation applies a transformation to the current value. */
public interface IStatCalculator
{
   int calculate( int currentValue );
}

// Base calculator — returns the value unchanged
public class BaseStatCalculator : IStatCalculator
{
   // Returns the current value with no modification
   public int calculate( int currentValue ) => currentValue;
}

/* Abstract decorator that wraps another IStatCalculator.
   Subclasses apply their own modification on top. */
public abstract class StatCalculatorDecorator : IStatCalculator
{
   protected IStatCalculator mInner;

   public StatCalculatorDecorator( IStatCalculator inner )
   {
      mInner = inner;
   }

   public abstract int calculate( int currentValue );
}

// Increases risk by 5 for a steal action
public class StealRiskDecorator : StatCalculatorDecorator
{
   public StealRiskDecorator( IStatCalculator inner ) : base( inner ) {}

   // Adds 5 risk for stealing
   public override int calculate( int currentValue ) => mInner.calculate( currentValue ) + 5;
}

// Increases risk by 10 for night fishing
public class NightFishRiskDecorator : StatCalculatorDecorator
{
   public NightFishRiskDecorator( IStatCalculator inner ) : base( inner ) {}

   // Adds 10 risk for night fishing
   public override int calculate( int currentValue ) => mInner.calculate( currentValue ) + 10;
}

// Increases risk by 5 for black market use
public class BlackMarketRiskDecorator : StatCalculatorDecorator
{
   public BlackMarketRiskDecorator( IStatCalculator inner ) : base( inner ) {}

   // Adds 5 risk for using the black market
   public override int calculate( int currentValue ) => mInner.calculate( currentValue ) + 5;
}

// Increases hunger by 20 for eating
public class EatHungerDecorator : StatCalculatorDecorator
{
   public EatHungerDecorator( IStatCalculator inner ) : base( inner ) {}

   // Adds 20 hunger for eating
   public override int calculate( int currentValue ) => mInner.calculate( currentValue ) + 20;
}

// Decreases hunger by 5 over time
public class TimeHungerDecorator : StatCalculatorDecorator
{
   public TimeHungerDecorator( IStatCalculator inner ) : base( inner ) {}

   // Removes 5 hunger as time passes
   public override int calculate( int currentValue ) => mInner.calculate( currentValue ) - 5;
}

// Increases health by 25 when healed
public class HealHealthDecorator : StatCalculatorDecorator
{
   public HealHealthDecorator( IStatCalculator inner ) : base( inner ) {}

   public override int calculate( int currentValue ) => mInner.calculate( currentValue ) + 25;
}

// Decreases health by 10 when hurt
public class HurtHealthDecorator : StatCalculatorDecorator
{
   public HurtHealthDecorator( IStatCalculator inner ) : base( inner ) {}

   // Removes 10 health when the player is hurt
   public override int calculate( int currentValue ) => mInner.calculate( currentValue ) - 10;
}

// Decreases health by a specific amount
public class TakeDamageDecorator : StatCalculatorDecorator
{
   private int mAmount;

   public TakeDamageDecorator( IStatCalculator inner, int amount ) : base( inner )
   {
      mAmount = amount;
   }

   // Removes the specified amount of health
   public override int calculate( int currentValue ) => mInner.calculate( currentValue ) - mAmount;
}
