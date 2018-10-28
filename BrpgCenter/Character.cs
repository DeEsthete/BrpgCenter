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
        [Key, DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
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
        public string AdvantagesDisadvantages { get; set; } //преимущества/недостатки
        public string Skills { get; set; } //способности
        public string Equip { get; set; } //экипировка
        #endregion
    }
}
