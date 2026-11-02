using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class Student
{
    public int OrderNo { get; set; }
    public string StudentName { get; set; } = "";
}

class Program
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        string filePath = "students.bin";

        // 1. Створення списку студентів
        List<Student> initialStudents = new List<Student>
        {
            new Student { OrderNo = 1, StudentName = "Овдійчук Марія" },
            new Student { OrderNo = 2, StudentName = "Белощук Максим" },
            new Student { OrderNo = 3, StudentName = "Стернійчук Олександра" },
            new Student { OrderNo = 4, StudentName = "Мороченець Ярослав" },
            new Student { OrderNo = 5, StudentName = "Струїнський Андрій" }
        };

        Console.WriteLine("=== Вихідний список студентів ===");
        foreach (var s in initialStudents)
        {
            Console.WriteLine($"№{s.OrderNo}: {s.StudentName}");
        }

        // 2. Запис списку в бінарний файл students.bin
        using (FileStream stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
        using (BinaryWriter writer = new BinaryWriter(stream))
        {
            foreach (var student in initialStudents)
            {
                WriteStudent(writer, student);
            }
        }

        FileInfo fileInfo = new FileInfo(filePath);
        Console.WriteLine($"\nДані успішно записано у файл: {filePath}");
        Console.WriteLine($"Розмір створеного файла: {fileInfo.Length} байт(ів)");

        // 3. Читання файла у новий список студентів
        List<Student> restoredStudents = new List<Student>();
        using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        using (BinaryReader reader = new BinaryReader(stream))
        {
            while (stream.Position < stream.Length)
            {
                Student student = ReadStudent(reader);
                restoredStudents.Add(student);
            }
        }

        // 4. Виведення відновленого списку студентів та порівняння
        Console.WriteLine("\n=== Відновлений список студентів із бінарного файла ===");
        foreach (var s in restoredStudents)
        {
            Console.WriteLine($"№{s.OrderNo}: {s.StudentName}");
        }

        // Перевірка відповідності
        bool isIdentical = true;
        if (initialStudents.Count == restoredStudents.Count)
        {
            for (int i = 0; i < initialStudents.Count; i++)
            {
                if (initialStudents[i].OrderNo != restoredStudents[i].OrderNo ||
                    initialStudents[i].StudentName != restoredStudents[i].StudentName)
                {
                    isIdentical = false;
                    break;
                }
            }
        }
        else
        {
            isIdentical = false;
        }

        Console.WriteLine(isIdentical ? "\n[OK] Відновлений список повністю збігається з вихідним!" : "\n[Помилка] Списки не збігаються!");
    }

    // Метод запису одного студента у бінарний потік
    static void WriteStudent(BinaryWriter writer, Student student)
    {
        writer.Write(student.OrderNo);
        byte[] nameBytes = Encoding.UTF8.GetBytes(student.StudentName);
        writer.Write(nameBytes.Length);
        if (nameBytes.Length > 0)
        {
            writer.Write(nameBytes);
        }
    }

    // Метод читання одного студента з бінарного потоку
    static Student ReadStudent(BinaryReader reader)
    {
        int orderNo = reader.ReadInt32();
        int nameLength = reader.ReadInt32();
        string studentName = string.Empty;
        if (nameLength > 0)
        {
            byte[] nameBytes = reader.ReadBytes(nameLength);
            studentName = Encoding.UTF8.GetString(nameBytes);
        }

        return new Student
        {
            OrderNo = orderNo,
            StudentName = studentName
        };
    }
}
