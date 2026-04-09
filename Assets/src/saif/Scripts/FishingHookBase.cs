using UnityEngine;

namespace Saif.GamePlay
{
    // TEACHER: This base class allows for Dynamic Binding.
    // The Selector script calls these methods without knowing the specific sub-type.
    public abstract class FishingHookBase : MonoBehaviour
    {
        public abstract void ResetHook();
        public abstract bool IsHookCast { get; }
    }
}