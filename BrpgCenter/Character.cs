using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrpgCenter
{
    public class Character
    {
        [Key]
        public int Id { get; set; }
        #region MainFields
        public string FullName { get; set; }

        [NotMapped]
        public Player Owner { get; set; } //хозяин 

        public string WorldName { get; set; } //Название мира в котором находится персонаж
        public int TL { get; set; } //Технический уровень
        public string Status { get; set; } //статус персонажа в мире
        public int Age { get; set; } //возраст
        public DateTime Birthday { get; set; } //дата рождения
        public string Eyes { get; set; } //описание глаз
        public string Hair { get; set; } //описание волос
        public string SkinColor { get; set; } //описание цвета кожи
        public string MainHand { get; set; } //основная рука
        public int Growth { get; set; } //рост
        public int Weight { get; set; } //вес
        public string Gender { get; set; } //пол
        public string Race { get; set; } //раса
        public string Religion { get; set; } //религия
        public int SM { get; set; } //модификатор размера
        public string MainNote { get; set; } //заметки
        #endregion
        #region Stats
        public int ST { get; set; } //сила
        public int DX{get; set;} //ловкость
        public int IQ{get; set;} //интелект
        public int HT{get; set;} //здоровье

        public int HP{get; set;} //жизни
        public int Move{get; set;} //движение
        public int Speed{get; set;} //скорость
        public int Will{get; set;} //воля
        public int Per{get; set;} //восприятие
        public int FP{get; set;} //усталость
        #endregion
        #region Other
        public string Wounds { get; set; } //раны
        public string Fatigue { get; set; } //усталость
        public IEnumerable<string> AdvantagesDisadvantages { get; set; } //преимущества/недостатки
        public IEnumerable<string> Skills { get; set; } //способности
        public IEnumerable<string> Magic { get; set; } //Заклинания
        public IEnumerable<string> Equip { get; set; } //экипировка
        public string SecondNote { get; set; } //заметка доп. полей
        #endregion

        #region Ctor
        public Character()
        {

        }
        public Character(int id, string fullName, Player owner, string worldName, int tL, string status, int age, DateTime birthday, string eyes, string hair, string skinColor, string mainHand, int growth, int weight, string gender, string race, string religion, int sM, string mainNote, int sT, int dX, int iQ, int hT, int hP, int move, int speed, int will, int per, int fP, string wounds, string fatigue, IEnumerable<string> advantagesDisadvantages, IEnumerable<string> skills, IEnumerable<string> magic, IEnumerable<string> equip, string secondNote)
        {
            Id = id;
            FullName = fullName ?? throw new ArgumentNullException(nameof(fullName));
            Owner = owner ?? throw new ArgumentNullException(nameof(owner));
            WorldName = worldName ?? throw new ArgumentNullException(nameof(worldName));
            TL = tL;
            Status = status ?? throw new ArgumentNullException(nameof(status));
            Age = age;
            Birthday = birthday;
            Eyes = eyes ?? throw new ArgumentNullException(nameof(eyes));
            Hair = hair ?? throw new ArgumentNullException(nameof(hair));
            SkinColor = skinColor ?? throw new ArgumentNullException(nameof(skinColor));
            MainHand = mainHand ?? throw new ArgumentNullException(nameof(mainHand));
            Growth = growth;
            Weight = weight;
            Gender = gender ?? throw new ArgumentNullException(nameof(gender));
            Race = race ?? throw new ArgumentNullException(nameof(race));
            Religion = religion ?? throw new ArgumentNullException(nameof(religion));
            SM = sM;
            MainNote = mainNote ?? throw new ArgumentNullException(nameof(mainNote));
            ST = sT;
            DX = dX;
            IQ = iQ;
            HT = hT;
            HP = hP;
            Move = move;
            Speed = speed;
            Will = will;
            Per = per;
            FP = fP;
            Wounds = wounds ?? throw new ArgumentNullException(nameof(wounds));
            Fatigue = fatigue ?? throw new ArgumentNullException(nameof(fatigue));
            AdvantagesDisadvantages = advantagesDisadvantages ?? throw new ArgumentNullException(nameof(advantagesDisadvantages));
            Skills = skills ?? throw new ArgumentNullException(nameof(skills));
            Magic = magic ?? throw new ArgumentNullException(nameof(magic));
            Equip = equip ?? throw new ArgumentNullException(nameof(equip));
            SecondNote = secondNote ?? throw new ArgumentNullException(nameof(secondNote));
        }
        #endregion
    }
}
