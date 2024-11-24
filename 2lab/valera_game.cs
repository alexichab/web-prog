using System;
using System.IO;
using Newtonsoft.Json;

class ValeraGame
{
    public class Valera
    {
        public int Health { get; set; }
        public int Mana { get; set; } // Алкоголь в крови
        public int Happiness { get; set; }
        public int Fatigue { get; set; }
        public int Money { get; set; }

        public Valera(int health, int mana, int happiness, int fatigue, int money)
        {
            Health = health;
            Mana = mana;
            Happiness = happiness;
            Fatigue = fatigue;
            Money = money;
        }

        public void NormalizeStats()
        {
            Health = Math.Clamp(Health, 0, 100);
            Mana = Math.Clamp(Mana, 0, 100);
            Happiness = Math.Clamp(Happiness, -10, 10);
            Fatigue = Math.Clamp(Fatigue, 0, 100);
        }
    }

    private static Valera valera = new Valera(100, 0, 5, 0, 50); // Инициализация
    private static dynamic config;
    static string configPath = "config.json";
    // static string savePath = "save.json";
    static string saveDirectory = "saves";

    static void Main(string[] args)
    {
        if (!Directory.Exists(saveDirectory))
        {
        Directory.CreateDirectory(saveDirectory);
        }
        LoadConfig();
        GameLoop();
    }

    static void GameLoop()
    {
        while (true)
        {
            Console.Clear();
            PrintStats();
            PrintActions();
            Console.WriteLine("Выберите действие (1-9):");

            if (int.TryParse(Console.ReadLine(), out int choice))
            {
                switch (choice)
                {
                    case 1: GoToWork(); break;
                    case 2: ContemplateNature(); break;
                    case 3: DrinkWineAndWatchSeries(); break;
                    case 4: GoToBar(); break;
                    case 5: DrinkWithMarginals(); break;
                    case 6: SingInMetro(); break;
                    case 7: Sleep(); break;
                    case 8: SaveGame(); break;
                    case 9: return; 
                    case 10: ReadTheBook(); break;
                    default: Console.WriteLine("Неверный выбор!"); break;
                }
            }
            else
            {
                Console.WriteLine("Некорректный ввод. Попробуйте еще раз.");
            }
            
            valera.NormalizeStats();
            if (valera.Health <= 0)
            {
                Console.WriteLine("Валера умер! Игра окончена.");
                break;
            }
        }
    }

    static void LoadConfig()
{
    string[] saveFiles = Directory.GetFiles(saveDirectory, "*.json");

    if (saveFiles.Length > 0)
    {
        Console.WriteLine("Найдены следующие сохранения:");
        for (int i = 0; i < saveFiles.Length; i++)
        {
            Console.WriteLine($"{i + 1}. {Path.GetFileNameWithoutExtension(saveFiles[i])}");
        }

        Console.WriteLine("Введите номер сохранения, которое хотите загрузить, или 0 для новой игры:");

        if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= saveFiles.Length)
        {
            string selectedSaveFile = saveFiles[choice - 1];
            string saveData = File.ReadAllText(selectedSaveFile);
            valera = JsonConvert.DeserializeObject<Valera>(saveData)!;
            Console.WriteLine($"Сохранение {Path.GetFileNameWithoutExtension(selectedSaveFile)} успешно загружено.");
            return;
        }
        else
        {
            Console.WriteLine("Начинаем новую игру.");
        }
    }
    else
    {
        Console.WriteLine("Сохранения не найдены. Начинаем новую игру.");
    }

    string configData = File.ReadAllText(configPath);
    config = JsonConvert.DeserializeObject(configData);
    valera = new Valera(
        (int)config.initialStats.health,
        (int)config.initialStats.mana,
        (int)config.initialStats.happiness,
        (int)config.initialStats.fatigue,
        (int)config.initialStats.money
    );
}

    static void SaveGame()
    {
    Console.WriteLine("Введите название для сохранения:");

    string? saveFileName = Console.ReadLine();
    if (!string.IsNullOrEmpty(saveFileName))
    {
        string saveFilePath = Path.Combine(saveDirectory, saveFileName + ".json"); // Путь для сохранения
        string saveData = JsonConvert.SerializeObject(valera, Formatting.Indented);
        File.WriteAllText(saveFilePath, saveData);
        Console.WriteLine($"Игра сохранена в файл {saveFilePath}!");
    }
    else
        {
        Console.WriteLine("Название для сохранения не может быть пустым.");
        }
    }   

   private static void PrintActions()
{
    Console.WriteLine("Доступные действия и их последствия:");

    // Пойти на работу
    Console.WriteLine("1. Пойти на работу:");
    Console.WriteLine($"   {config.actions.goToWork.happiness} Жизнерадостность, " +
                      $"{config.actions.goToWork.mana} Алкоголь, " +
                      $"{config.actions.goToWork.money}$, " +
                      $"{config.actions.goToWork.fatigue} Усталость");

    // Созерцать природу
    Console.WriteLine("2. Созерцать природу:");
    Console.WriteLine($"   +{config.actions.contemplateNature.happiness} Жизнерадостность, " +
                      $"{config.actions.contemplateNature.mana} Алкоголь, " +
                      $"{config.actions.contemplateNature.fatigue} Усталость");

    // Пить вино и смотреть сериал
    Console.WriteLine("3. Пить вино и смотреть сериал:");
    Console.WriteLine($"   {config.actions.drinkWineAndWatchSeries.happiness} Жизнерадостность, " +
                      $"{config.actions.drinkWineAndWatchSeries.mana} Алкоголь, " +
                      $"{config.actions.drinkWineAndWatchSeries.fatigue} Усталость, " +
                      $"{config.actions.drinkWineAndWatchSeries.health} Здоровье, " +
                      $"{config.actions.drinkWineAndWatchSeries.money}$");

    // Сходить в бар
    Console.WriteLine("4. Сходить в бар:");
    Console.WriteLine($"   {config.actions.goToBar.happiness} Жизнерадостность, " +
                      $"{config.actions.goToBar.mana} Алкоголь, " +
                      $"{config.actions.goToBar.fatigue} Усталость, " +
                      $"{config.actions.goToBar.health} Здоровье, " +
                      $"{config.actions.goToBar.money}$");

    // Выпить с маргинальными личностями
    Console.WriteLine("5. Выпить с маргинальными личностями:");
    Console.WriteLine($"   {config.actions.drinkWithMarginals.happiness} Жизнерадостность, " +
                      $"{config.actions.drinkWithMarginals.mana} Алкоголь, " +
                      $"{config.actions.drinkWithMarginals.fatigue} Усталость, " +
                      $"{config.actions.drinkWithMarginals.health} Здоровье, " +
                      $"{config.actions.drinkWithMarginals.money}$");

    // Петь в метро
    Console.WriteLine("6. Петь в метро:");
    Console.WriteLine($"   {config.actions.singInMetro.happiness} Жизнерадостность, " +
                      $"{config.actions.singInMetro.mana} Алкоголь, " +
                      $"{config.actions.singInMetro.money}$, " +
                      $"{config.actions.singInMetro.fatigue} Усталость");
    Console.WriteLine($"   +{config.actions.singInMetro.bonusMoney}$ если Алкоголь > " +
                      $"{config.actions.singInMetro.bonusMoneyIfManaInRange[0]} и < " +
                      $"{config.actions.singInMetro.bonusMoneyIfManaInRange[1]}");

    // Спать
    Console.WriteLine("7. Спать:");
    Console.WriteLine($"   +{config.actions.sleep.healthBonus} Здоровье, если Алкоголь < " +
                      $"{config.actions.sleep.healthBonusIfManaBelow}");
    Console.WriteLine($"   -{config.actions.sleep.happinessPenalty} Жизнерадостность, если Алкоголь > " +
                      $"{config.actions.sleep.happinessPenaltyIfManaAbove}");
    Console.WriteLine($"   {config.actions.sleep.mana} Алкоголь, " +
                      $"{config.actions.sleep.fatigue} Усталость");
    Console.WriteLine("8. Сохранить игру.");
    Console.WriteLine("9. Выйти из игры.");
    Console.WriteLine("10.Читать книгу");
}



   static void GoToWork()
{
    var workConfig = config.actions.goToWork;

    if (valera.Mana < 50 && valera.Fatigue < 10)
    {
        valera.Happiness += (int)workConfig.happiness;
        valera.Mana += (int)workConfig.mana;
        valera.Money += (int)workConfig.money;
        valera.Fatigue += (int)workConfig.fatigue;
        Console.WriteLine("Вы пошли на работу.");
    }
    else
    {
        Console.WriteLine("Валера слишком пьян или устал, чтобы пойти на работу.");
    }
    Console.ReadLine();
}

static void ContemplateNature()
{
    var natureConfig = config.actions.contemplateNature;
    valera.Happiness += (int)natureConfig.happiness;
    valera.Mana += (int)natureConfig.mana;
    valera.Fatigue += (int)natureConfig.fatigue;
    Console.WriteLine("Вы созерцали природу.");
    Console.ReadLine();
}

static void DrinkWineAndWatchSeries()
{
    var seriesConfig = config.actions.drinkWineAndWatchSeries;
    valera.Happiness += (int)seriesConfig.happiness;
    valera.Mana += (int)seriesConfig.mana;
    valera.Fatigue += (int)seriesConfig.fatigue;
    valera.Health += (int)seriesConfig.health;
    valera.Money += (int)seriesConfig.money;
    Console.WriteLine("Вы пили вино и смотрели сериал.");
    Console.ReadLine();
}

static void GoToBar()
{
    var barConfig = config.actions.goToBar;
    valera.Happiness += (int)barConfig.happiness;
    valera.Mana += (int)barConfig.mana;
    valera.Fatigue += (int)barConfig.fatigue;
    valera.Health += (int)barConfig.health;
    valera.Money += (int)barConfig.money;
    Console.WriteLine("Вы сходили в бар.");
    Console.ReadLine();
}

static void DrinkWithMarginals()
{
    var marginalsConfig = config.actions.drinkWithMarginals;
    valera.Happiness += (int)marginalsConfig.happiness;
    valera.Mana += (int)marginalsConfig.mana;
    valera.Fatigue += (int)marginalsConfig.fatigue;
    valera.Health += (int)marginalsConfig.health;
    valera.Money += (int)marginalsConfig.money;
    Console.WriteLine("Вы выпили с маргиналами.");
    Console.ReadLine();
}

static void ReadTheBook()
{
    var BookConfig = config.actions.book;
    valera.Happiness +=(int)BookConfig.happiness;
    valera.Fatigue += (int)BookConfig.fatigue;
    valera.Mana +=(int)BookConfig.mana;
    Console.WriteLine("Вы почитали книгу");
}

static void SingInMetro()
{
    var metroConfig = config.actions.singInMetro;
    valera.Happiness += (int)metroConfig.happiness;
    valera.Mana += (int)metroConfig.mana;
    valera.Fatigue += (int)metroConfig.fatigue;
    valera.Money += (int)metroConfig.money;

    // Бонус за уровень алкоголя
    if (valera.Mana > (int)metroConfig.bonusMoneyIfManaInRange[0] && valera.Mana < (int)metroConfig.bonusMoneyIfManaInRange[1])
    {
        valera.Money += (int)metroConfig.bonusMoney;
    }

    Console.WriteLine("Вы пели в метро.");
    Console.ReadLine();
}

static void Sleep()
{
    var sleepConfig = config.actions.sleep;

    if (valera.Mana < (int)sleepConfig.healthBonusIfManaBelow)
    {
        valera.Health += (int)sleepConfig.healthBonus;
    }
    if (valera.Mana > (int)sleepConfig.happinessPenaltyIfManaAbove)
    {
        valera.Happiness += (int)sleepConfig.happinessPenalty;
    }
    valera.Mana += (int)sleepConfig.mana;
    valera.Fatigue += (int)sleepConfig.fatigue;

    Console.WriteLine("Вы спали.");
    Console.ReadLine();
}
    static void PrintStats()
    {
        Console.WriteLine($"Здоровье: {valera.Health}");
        Console.WriteLine($"Алкоголь в крови: {valera.Mana}");
        Console.WriteLine($"Жизнерадостность: {valera.Happiness}");
        Console.WriteLine($"Усталость: {valera.Fatigue}");
        Console.WriteLine($"Деньги: {valera.Money}");
        Console.WriteLine();
    }
}
