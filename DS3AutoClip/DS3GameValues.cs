using System;
using System.Reflection;

namespace DS3AutoClip
{
    public class DS3GameValues
    {
        // Player stats

        public readonly ObvservedValue<int> PlayerHP = new ObvservedValue<int>(
            GameProcess.WorldChrMan,
            addr => addr.Deref()
                .Deref(offset: 0x80)
                .Deref(offset: 0x1f90)
                .Deref(offset: 0x18)
                .Offset(0xd8)
        );
        public readonly ObvservedValue<int> PlayerCharacterType = new ObvservedValue<int>(
            GameProcess.WorldChrMan,
            addr => addr.Deref()
                .Deref(offset: 0x80)
                .Offset(0x70)
        );
        public readonly ObvservedValue<int> PlayerTeamType = new ObvservedValue<int>(
            GameProcess.WorldChrMan,
            addr => addr.Deref()
                .Deref(offset: 0x80)
                .Offset(0x74)
        );
        public readonly ObvservedValue<byte> PlayerHollowing = new ObvservedValue<byte>(
            GameProcess.GameMan,
            addr => addr.Deref()
                .Offset(0x204e)
        );

        // Misc

        public readonly ObvservedValue<byte> IsCollisionEnabled = new ObvservedValue<byte>(
            GameProcess.FieldArea,
            addr => addr.Deref()
                .Deref(offset: 0x60)
                .Offset(0x48)
        );

        public DerivedValue<byte, bool> IsLevelLoaded { get => IsCollisionEnabled.Derive(v => v != null); }
        public DerivedValue<byte, bool> IsTitleScreen { get => PlayerHollowing.Derive(v => v == null || v == 255); }

        // Multiplayer

        public readonly ObvservedValue<int> AreaForOnlineActivity = new ObvservedValue<int>(
            GameProcess.WorldChrMan,
            addr => addr.Deref()
                .Deref(offset: 0x80)
                .Offset(0x1abc)
        );
        public readonly ObvservedValue<int> InvasionType = new ObvservedValue<int>(
            GameProcess.GameMan,
            addr => addr.Deref()
                .Offset(0xC54)
        );

        public void Update(GameProcess game)
        {
            var valueFields = typeof(DS3GameValues).GetFields(BindingFlags.Instance | BindingFlags.Public);
            foreach (var field in valueFields)
            {
                var obsValue = field.GetValue(this);
                var ovType = obsValue.GetType();
                var updateMethod = ovType.GetMethod("Update");
                updateMethod.Invoke(obsValue, new object[] { game });
            }
        }
    }

    public class ObvservedValue<T>
        where T : struct
    {
        public T? Value = null;
        public T? Prev = null;

        private readonly SymbolData symbol;
        private readonly Func<GameProcess.Addr, GameProcess.Addr> addrFn;

        public ObvservedValue(SymbolData symbol, Func<GameProcess.Addr, GameProcess.Addr> addrFn)
        {
            this.symbol = symbol;
            this.addrFn = addrFn;
        }

        private T? GetFreshValue(GameProcess game)
        {
            if (game == null)
                return null;

            var addr = addrFn(game.AddressOf(symbol));
            if (!addr.IsValid)
                return null;

            try
            {
                return addr.Read<T>();
            }
            catch
            {
                return null;
            }
        }

        public void Update(GameProcess game)
        {
            var newValue = GetFreshValue(game);
            Prev = Value;
            Value = newValue;
        }

        public DerivedValue<T, R> Derive<R>(Func<T?, R> derive) where R : struct
        {
            return new DerivedValue<T, R>(this, derive);
        }
    }

    public readonly struct DerivedValue<T, R>
        where T : struct
        where R : struct
    {
        private readonly ObvservedValue<T> value;
        private readonly Func<T?, R> derive;

        public R Value { get => derive(value.Value); }
        public R Prev { get => derive(value.Prev); }

        public DerivedValue(ObvservedValue<T> value, Func<T?, R> derive)
        {
            this.value = value;
            this.derive = derive;
        }
    }
}
