using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SkilledAtSitting
{
    using SideLoader;
    using UnityEngine;

    class Sitting : Effect
    {
        public const float TICK_RATE = 1;
        protected override void ActivateLocally(Character _affectedCharacter, object[] _infos)
        {
            if ((((_affectedCharacter?.Animator?.velocity != null) && (_affectedCharacter.Animator.velocity.sqrMagnitude > 0.1))|| ((_affectedCharacter?.AnimMoveSqMagnitude ?? 0) > 0.1 && this.m_parentStatusEffect.Age > 0.5)) && this.m_parentStatusEffect.Age > 1)
            {
                _affectedCharacter.StatusEffectMngr.CleanseStatusEffect(SkilledAtSitting.SITTING_EFFECT_NAME);
            }

            float ticksPerGameHour = 3600f / 24f / TICK_RATE;
            float effectiveness = 0.75f;

            _affectedCharacter.Stats.AffectHealth(35 / ticksPerGameHour * effectiveness);
            _affectedCharacter.Stats.RestoreBurntHealth(0.05f / ticksPerGameHour * effectiveness, true);
            _affectedCharacter.Stats.RestoreBurntStamina(20 / ticksPerGameHour * effectiveness, false);
        }
    }
}
