using System;
using System.IO;
using Newtonsoft.Json;
using System.Xml.Serialization;
using System.Xml;

public class Figure
{
    public string Name { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }

    public Figure() { } // Конструктор для десериализации
    public Figure(string name, int width, int height)
    {
        Name = name;
        Width = width;
        Height = height;
    }
}

public class FileHandler
{
    private string filePath;

    public FileHandler(string path)
    {
        filePath = path;
    }

    public void LoadData(out Figure[] figures)
    {
        try
        {
            string extension = Path.GetExtension(filePath);

            if (extension.Equals(".txt"))
            {
                LoadFromTxt(out figures);
            }
            else if (extension.Equals(".json"))
            {
                LoadFromJson(out figures);
            }
            else if (extension.Equals(".xml"))
            {
                LoadFromXml(out figures);
            }
            else
            {
                throw new NotSupportedException("Unsupported file format.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading data: {ex.Message}");
            figures = null;
        }
    }

    public void SaveData(Figure[] figures)
    {
        try
        {
            string extension = Path.GetExtension(filePath);

            if (extension.Equals(".txt"))
            {
                SaveToTxt(figures);
            }
            else if (extension.Equals(".json"))
            {
                SaveToJson(figures);
            }
            else if (extension.Equals(".xml"))
            {
                SaveToXml(figures);
            }
            else
            {
                throw new NotSupportedException("Unsupported file format.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving data: {ex.Message}");
        }
    }

    private void LoadFromTxt(out Figure[] figures)
    {
        string[] lines = File.ReadAllLines(filePath);
        figures = new Figure[lines.Length];

        for (int i = 0; i < lines.Length; i++)
        {
            string[] parts = lines[i].Split(',');
            figures[i] = new Figure(parts[0], int.Parse(parts[1]), int.Parse(parts[2]));
        }
    }

    private void LoadFromJson(out Figure[] figures)
    {
        string jsonContent = File.ReadAllText(filePath);
        figures = JsonConvert.DeserializeObject<Figure[]>(jsonContent);
    }

    private void LoadFromXml(out Figure[] figures)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(Figure[]));

        using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
        {
            figures = (Figure[])serializer.Deserialize(fileStream);
        }
    }

    private void SaveToTxt(Figure[] figures)
    {
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            foreach (Figure figure in figures)
            {
                writer.WriteLine($"{figure.Name},{figure.Width},{figure.Height}");
            }
        }
    }

    private void SaveToJson(Figure[] figures)
    {
        string jsonContent = JsonConvert.SerializeObject(figures, Formatting.Indented);
        File.WriteAllText(filePath, jsonContent);
    }

    private void SaveToXml(Figure[] figures)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(Figure[]));

        using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
        {
            serializer.Serialize(fileStream, figures);
        }
    }
}

public class TextEditor
{
    private Figure[] figures;
    private FileHandler fileHandler;

    public TextEditor(string filePath)
    {
        fileHandler = new FileHandler(filePath);
        fileHandler.LoadData(out figures);
    }

    public void Run()
    {
        Console.WriteLine("Text Editor - Press F1 to save, Escape to exit");

        while (true)
        {
            DisplayFigures();

            ConsoleKeyInfo key = Console.ReadKey();

            if (key.Key == ConsoleKey.Escape)
            {
                break;
            }
            else if (key.Key == ConsoleKey.F1)
            {
                fileHandler.SaveData(figures);
                Console.WriteLine("File saved successfully.");
            }
        }
    }

    private void DisplayFigures()
    {
        Console.Clear();
        Console.WriteLine("Figures:");

        foreach (Figure figure in figures)
        {
            Console.WriteLine($"Name: {figure.Name}, Width: {figure.Width}, Height: {figure.Height}");
        }
    }
}

class Program
{
    static void Main()
    {
        Console.WriteLine("Enter the path of the file:");
        string filePath = Console.ReadLine();

        TextEditor textEditor = new TextEditor(filePath);
        textEditor.Run();
    }
}
