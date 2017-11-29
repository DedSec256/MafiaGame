using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mafiagame.DataLayer.Models;

namespace MafiaGame.DataLayer.Models
{
    public enum Roles
    {
        Citizen,
        Mafia,
        Comissar,
        Doctor,
        Maniac,
        Dead
    }

    public abstract class Role
    {
        public readonly Roles _role;
        public readonly long _user;

        protected Role(Roles role, long user)
        {
            _role = role;
            _user = user;
        }

        public void DiscussAction()
        {

        }

        public void VotingAction()
        {

        }

        public virtual void NightAction()
        {

        }

        public override string ToString()
        {
            return "Добро пожаловать в мафию!\n" +
                   "Сейчас каждому учатнику будет открыта его роль";
        }
    }

    public class CitizenRole : Role
    {
        public CitizenRole(long player) : base(Roles.Citizen, player)
        {
        }

        public override string ToString()
        {
            return base.ToString() + "Ты мафия";
        }
    }

    public class MafiaRole : Role
    {
        public MafiaRole(long player) : base(Roles.Mafia, player)
        {
        }

        public override string ToString()
        {
            return base.ToString() + "Ты мафия";
        }
    }

    public class ManiacRole : Role
    {
        public ManiacRole(long player) : base(Roles.Maniac, player)
        {
        }

        public override string ToString()
        {
            return base.ToString() + "Ты маньяк";
        }
    }

    public class DoctorRole : Role
    {
        public DoctorRole(long player) : base(Roles.Doctor, player)
        {
        }

        public override string ToString()
        {
            return base.ToString() + "Ты доктор";
        }
    }

    public class ComissarRole : Role
    {
        public ComissarRole(long player) : base(Roles.Comissar, player)
        {
        }

        public override string ToString()
        {
            return base.ToString() + "Ты комиссар";
        }
    }

    public class DeadRole : Role
    {
        public DeadRole(long player) : base(Roles.Dead, player)
        {
        }

        public override string ToString()
        {
            return base.ToString() + "Тебя убили :c";
        }
    }

    public static class RoleFactory
    {
        public static Role GetRole(PlayersRooms playerRoom)
        {
            Roles.TryParse(playerRoom.Role, out Roles role);

            switch (role)
            {
                case Roles.Citizen:
                    return new CitizenRole(playerRoom.UserId);
                case Roles.Comissar:
                    return new ComissarRole(playerRoom.UserId);
                case Roles.Dead:
                    return new DeadRole(playerRoom.UserId);
                case Roles.Doctor:
                    return new DoctorRole(playerRoom.UserId);
                case Roles.Mafia:
                    return new MafiaRole(playerRoom.UserId);
                case Roles.Maniac:
                    return new ManiacRole(playerRoom.UserId);
            }
            return null;
        }

        public static string GetRoleIcon(this Roles role)
        {
            switch (role)
            {
                case Roles.Citizen:
                    return "👨";
                case Roles.Comissar:
                    return "🚔";
                case Roles.Dead:
                    return "😵";
                case Roles.Doctor:
                    return "💉";
                case Roles.Mafia:
                    return "💣";
                case Roles.Maniac:
                    return "🔪";
            }
            return "❌";
        }
    }
}
