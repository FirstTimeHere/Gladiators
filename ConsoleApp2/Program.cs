using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.IO;


namespace ConsoleApp2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Coliseum coliseum = new Coliseum();
            coliseum.Work();
        }
    }

    class Coliseum
    {
        public void Work()
        {
            bool isAlive = true;
            bool isProgrammWork = true;

            const string CommandFight = "1";
            const string CommandExit = "2";

            List<Human> humans = new List<Human>();

            Warrior warrior = new Warrior("Воин", 100, 2000);
            Warrior warriorSecond = new Warrior("Второй воин", 150, 1500);
            Shooter shooter = new Shooter("Стрелок", 70, 1000);
            Paladin paladin = new Paladin("Паладин", 150, 2000);
            Mage mage = new Mage("Маг", 50, 1000, 500);

            humans.Add(warrior);
            humans.Add(warriorSecond);
            humans.Add(shooter);
            humans.Add(paladin);
            humans.Add(mage);

            while (isProgrammWork)
            {
                Console.Clear();

                Console.Write($"Для боя нажмите: {CommandFight}\n" +
                    $"Для выхода нажмите: {CommandExit}\n" +
                    "\nВведите команду:");
                string userInputSwitchChoice = Console.ReadLine();

                switch (userInputSwitchChoice)
                {
                    case CommandFight:
                        Fight(isAlive, humans.ToArray());
                        break;
                    case CommandExit:
                        isProgrammWork = false;
                        Console.WriteLine("Программа завершила работу");
                        break;
                    default:
                        ShowMessage();
                        break;
                }
            }
        }

        private void Fight(bool isAlive, Human[] humans)
        {
            isAlive = true;

            Console.Clear();
            Console.WriteLine("Бой начинается!");
            Console.Write("Выбор первого гладиатора: ");
            if (int.TryParse(Console.ReadLine(), out int userInputFirstChoice))
            {
                userInputFirstChoice -= 1;
                Console.Write("\nВыбор второго гладиатора: ");
                if (int.TryParse(Console.ReadLine(), out int userInputSecondChoice))
                {
                    userInputSecondChoice -= 1;

                    if (userInputFirstChoice == userInputSecondChoice)
                    {
                        ShowMessage("Боец не должен бить самого себя!");
                    }
                    else
                    {
                        ShowHealthFighters(humans.ToArray(), userInputFirstChoice, userInputSecondChoice);

                        while (isAlive)
                        {
                            humans[userInputFirstChoice].TakeDamage(humans[userInputSecondChoice].GetFullDamage());
                            humans[userInputSecondChoice].TakeDamage(humans[userInputFirstChoice].GetFullDamage());

                            ShowHealthFighters(humans.ToArray(), userInputFirstChoice, userInputSecondChoice);

                            if (humans[userInputFirstChoice].Health <= 0 && humans[userInputSecondChoice].Health <= 0 )
                            {
                                Console.WriteLine($"Оба игрока погибли" +
                                    $"\nНичья!");
                                isAlive = false;
                                Console.ReadKey();
                            }
                            else if (humans[userInputFirstChoice].Health <= 0)
                            {
                                Console.WriteLine($"\n{humans[userInputSecondChoice].Name} победил");
                                isAlive = false;
                                Console.ReadKey();
                            }
                            else if (humans[userInputSecondChoice].Health <= 0)
                            {
                                isAlive = false;
                                Console.WriteLine($"\n{humans[userInputFirstChoice].Name} победил");
                                Console.ReadKey();
                            }
                        }
                    }
                }
                else
                {
                    ShowMessage();
                }
            }
            else
            {
                ShowMessage();
            }
        }

        private void ShowHealthFighters(Human[] humans, int firstFighter, int secondFighter)
        {
            Console.Write($"\n{humans[firstFighter].Health} - Здоровье {humans[firstFighter].Name}\n" +
                                       $"{humans[secondFighter].Health} - Здоровье {humans[secondFighter].Name}\n");
        }

        private void ShowMessage(string error = "Неверный формат")
        {
            Console.Write($"\n{error}" +
                "\nДля продолжения нажмите любую клавишу\n");
            Console.ReadKey();
        }
    }

    abstract class Human
    {
        private static int s_id = 0;
        private static Random s_random = new Random();


        public Human()
        {
            Id = ++s_id;
        }

        public static int GetRandomValue(int minRandomValue, int maxRandomValue)
        {
            return s_random.Next(minRandomValue, maxRandomValue);
        }

        private int Id { get; set; }

        public string Name { get; protected set; }

        public float Damage { get; protected set; }

        public float Health { get; protected set; }

        public virtual void TakeDamage(float damage)
        {
            Health -= damage;
        }

        public virtual float GetFullDamage()
        {
            return Damage;
        }
    }

    class Warrior : Human
    {
        private static int s_minAromorValue = 1;
        private static int s_maxAromorValue = 40;
        private int _armor = GetRandomValue(s_minAromorValue, s_maxAromorValue);

        public Warrior(string name, float defaultDamage, float defaulHealth)
        {
            Name = name;
            Health = defaulHealth;
            Damage = defaultDamage;
        }

        public override void TakeDamage(float damage)
        {
            if (_armor < 0)
            {
                Health -= (damage * (-1 * _armor));
            }
            else if (_armor > 0)
            {
                Health -= (damage / _armor);
            }
            else
            {
                Health -= damage;
            }
        }

        public override float GetFullDamage()
        {
            return base.GetFullDamage();
        }
    }

    class Spell
    {
        public Spell(string name, int damage, int manaCost)
        {
            Name = name;
            Damage = damage;
            ManaCost = manaCost;
        }

        public string Name { get; private set; }

        public int Damage { get; private set; }

        public int ManaCost { get; private set; }
    }

    class Mage : Human
    {
        private List<Spell> _spells = new List<Spell>();

        public Mage(string name, float defaultDamage, float defaulHealth, int mana)
        {
            Name = name;
            Health = defaulHealth;
            Damage = defaultDamage;
            Mana = mana;

            AddSpells();
        }

        public int Mana { get; private set; }

        public override float GetFullDamage()
        {
            ChoiceSpell(out Spell spell);

            Console.Write($"\nЗаклинание: {spell.Name}");

            if (spell.ManaCost > Mana)
            {
                return Damage;
            }
            else if (spell.Name == "Восстановление маны")
            {
                Mana += spell.Damage;

                return Damage;
            }
            else
            {
                Damage += spell.Damage;
                Mana -= spell.ManaCost;

                return Damage;
            }
        }

        private void AddSpells()
        {
            _spells.Clear();

            _spells.Add(new Spell("Ледяной шип", 100, 30));
            _spells.Add(new Spell("Огненный шар", 250, 50));
            _spells.Add(new Spell("Восстановление маны", 10, 0));
            _spells.Add(new Spell("ВЗРЫВ", 10000, 1000));
            _spells.Add(new Spell("Порча", 300, 150));
        }

        private void ChoiceSpell(out Spell spell)
        {
            spell = _spells[GetRandomValue(0, _spells.Count)];
        }
    }

    class Paladin : Human
    {
        private int _resurrectionHealth = 125;
        private float _criticalDamage = 10f;
        private bool isPrayActive = false;

        public Paladin(string name, float defaultDamage, float defaulHealth)
        {
            Name = name;
            Health = defaulHealth;
            Damage = defaultDamage;
        }

        public override void TakeDamage(float damage)
        {
            if (Health < 1000)
            {
                Pray();
                base.TakeDamage(damage);
            }
            else
            {
                isPrayActive = false;
                base.TakeDamage(damage);
            }

        }

        public override float GetFullDamage()
        {
            if (isPrayActive)
            {
                return Damage * _criticalDamage;
            }
            else
            {
                return base.GetFullDamage();
            }
        }

        private void Pray()
        {
            Console.WriteLine("Активирована особая способность: Мольба");
            Health += _resurrectionHealth;
            isPrayActive = true;
        }
    }

    class Shooter : Human
    {
        private int _fullClip = 5;
        private int _reloadTime = 3;
        private float _damageOneBullet = 5f;

        public Shooter(string name, float defaultDamage, float defaulHealth)
        {
            Name = name;
            Health = defaulHealth;
            Damage = defaultDamage;
        }

        public override float GetFullDamage()
        {
            if (Shoot())
            {
                return Damage * _damageOneBullet;
            }
            else
            {
                Console.WriteLine("перезарядка");
                return base.GetFullDamage();
            }
        }

        private bool Shoot()
        {
            if (Reload() == false)
            {
                _fullClip--;
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool Reload()
        {
            if (_fullClip == 0 && _reloadTime > 0)
            {
                _reloadTime--;
                return true;
            }
            else if (_reloadTime == 0)
            {
                _reloadTime = 3;
                _fullClip = 5;
                return false;
            }
            else
            {
                return false;
            }
        }
    }
}
