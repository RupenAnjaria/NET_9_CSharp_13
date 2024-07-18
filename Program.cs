using System.Buffers;
using System.Diagnostics;
using Console9;
using System.Collections;

public class Student
{
    public string Name { get; set; }
    public string Major { get; set; }
    public int Fees { get; set; }
}

//Try below code in SharpLab.io for Semi-auto property
//https://sharplab.io/#v2:EYLgZgpghgLgrgJwgZwLTIgWwJaqnGAe1QAcFCTkAaAExAGoAfIA
/////////////START
//using System;

class Employee
{
    public int Age { get; set; }
}

class Employee_
{
    private int _age;
    public int Age
    {
        get { return _age; }
        set
        {
            if (value < 18)
            {
                throw new ArgumentOutOfRangeException("must be > 18");
            }
            _age = value;
        }
    }
}

class Employee9
{

    public int Age
    {

        get => field;

        set
        {
            if (value < 18)
            {
                throw new ArgumentOutOfRangeException("must be > 18");
            }
            field = value;
        }
    }

}

class Employee8
{

    private int field;

    public int Field
    {

        get => field;

        set
        {
            if (value < 18)
            {
                throw new ArgumentOutOfRangeException("must be > 18");
            }
            field = value;
        }
    }

}

public class Program
{
    static void Main()
    {
        Employee8 e8 = new Employee8();
        e8.Field = 99;
        Console.WriteLine(e8.Field);
    }
}

class Test
{
    private int @field;
    public int Field
    {
        get { return @field; }
        set { @field = value; }
    }
}

/////////////END



public class MyNumbers : IEnumerable<int>
{
    private readonly List<int> _numbers = new();

    public IEnumerator<int> GetEnumerator()
    {
        return _numbers.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Add(int num)
    {
        _numbers.Add(num);
    }
}

class Program
{

    static void Main()
    {

        #region LINQ
        var students = new List<Student>
        {
            new Student {Fees = 20, Name="Steve", Major = "Maths", },
            new Student {Fees = 10, Name="Bob", Major = "Physics"},
            new Student {Fees = 15, Name="David", Major = "Chemistry"},
            new Student {Fees = 15, Name="Evan", Major = "Chemistry"},
            new Student {Fees = 15, Name="Ken", Major = "Chemistry"},
            new Student {Fees = 10, Name="Michael", Major = "Physics"},
            new Student {Fees = 10, Name="Lisa", Major = "Physics"},
            new Student {Fees = 20, Name="Jenn", Major = "Maths"},
            new Student {Fees = 20, Name="Yen", Major = "Maths"},
            new Student {Fees = 20, Name="Xu", Major = "Maths"},
            new Student {Fees = 5, Name="P", Major = "Biology"},
        };

        //.NET 8
        var majorCounts = students
            .GroupBy(user => user.Major)
            .Select(group => new { Major = group.Key, Count = group.Count() }); // Select the role and count for each group


        foreach (var majorCount in majorCounts)
        {
            Console.WriteLine($"There are {majorCount.Count} students with major as {majorCount.Major}");
        }

        // .NET 9
        foreach (var studentCount in students.CountBy(student => student.Major))
        {
            Console.WriteLine($"There are {studentCount.Value} students with major as {studentCount.Key}");
        }


        //MaxBy
        var maxMajor = students.CountBy(student => student.Major)
                            .MaxBy(student => student.Value);

        Console.WriteLine($"Most student choose {maxMajor.Key}");


        //MinBy
        var minMajor = students.CountBy(student => student.Major)
                            .MinBy(student => student.Value);

        Console.WriteLine($"Few student choose {minMajor.Key}");


        //AggregateBy
        var sumFeesByMajor = students.AggregateBy(
        s => s.Major,
        seed: 0,
        (cTotal, student) => cTotal + student.Fees);


        foreach (var roleAggregate in sumFeesByMajor)
        {
            Console.WriteLine($"Total of Fees for {roleAggregate.Key} is {roleAggregate.Value}");
        }

        //IEnumerable.Index()
        //.NET 8
        foreach (var (v, i) in students.Select((i, v) => (i, v)))
        {
            Console.WriteLine($"index is {i} value is {v.Name}");
        }

        //.NET 9
        foreach (var (i, v) in students.Index())
        {
            Console.WriteLine($"index is {i} value is {v.Name}");
        }


        #endregion

        #region SearchValues (string)

        //.net 8 class SearchValues gets better by accepting string (only char, byte in .NET 8)
        /*
         * Fastest way to search string in an string array
         * It does the calculation on what search method is best. The actual method is not documented
         * but it depends on the hardware (CPU vector) and the source array
         */

        //8 example
        char[] charArr = { 'q', 'f', 'z' };
        SearchValues<char> sourceString8 = SearchValues.Create(charArr);
        Console.WriteLine(sourceString8.Contains('f'));


        //9 example
        string[] sourceStringArr = ["The","quick", "brown", "fox", "jumps", "over", "the",
                        "lazy","dog."];

            Stopwatch stopwatch1 = new Stopwatch();
            stopwatch1.Start();

        Console.WriteLine(sourceStringArr.Contains("BROWN", StringComparer.OrdinalIgnoreCase));
        stopwatch1.Stop();
        Console.WriteLine($"String Array (Elapsed time: {stopwatch1.ElapsedTicks:N0} ticks)");

            stopwatch1.Reset();
            stopwatch1.Start();

        SearchValues<string> sourceString = SearchValues
                                .Create(sourceStringArr, StringComparison.OrdinalIgnoreCase);


            stopwatch1.Start();
        Console.WriteLine(sourceString.Contains("BROWN"));
        //Console.WriteLine(sv.Contains("a wild fox")); // false
            stopwatch1.Stop();
        Console.WriteLine($"String Value (Elapsed time: {stopwatch1.ElapsedTicks:N0} ticks)");

            stopwatch1.Reset();


        #endregion

        #region AsynAwait

        static async Task<string> SimulateDelayAsync(int delay, string taskName)
        {
            await Task.Delay(delay);
            return taskName;
        }

        var task = Task.Run(() => RunTasksAsync());

        task.Wait();


        static async Task RunTasksAsync()
        {
            // Create a list of tasks with different delays
            List<Task<string>> tasks = [

            SimulateDelayAsync(4000, "Task 1"),
            SimulateDelayAsync(3000, "Task 2"),
            SimulateDelayAsync(2000, "Task 3"),
            SimulateDelayAsync(1000, "Task 4")
            ];

            // Wait for all tasks to complete
            var outputs = await Task.WhenAll(tasks);

            foreach (var result in outputs)
            {
                Console.WriteLine($"{result} completed");
            }

            //.NET 8 way
            while (tasks.Any())
            {
                var ft = await Task.WhenAny(tasks);
                tasks.Remove(ft);
                Console.WriteLine(await ft);
            }

            //.NET 9 way
            await foreach (var task in Task.WhenEach(tasks))
            {
                Console.WriteLine(task.Result);
            }
        }


        #endregion

        #region HybridCache

        /* We need to install a Nuget Package "Microsoft.Extensions.Caching.Hybrid"
         * HybridCache has primary and secondary cache OOB, which we have to implement on our own in .8
         * it has build-in stamped protection
         */
        var taskh = Task.Run(() => RunHTasksAsync());

        taskh.Wait();

        static async Task RunHTasksAsync()
        {
            HCaching hCaching = new HCaching();
            var s = await hCaching.NoCache();
            Console.WriteLine(s);

            var d = await hCaching.GetFromDistributedCache();
            Console.WriteLine(d);

            var m = await hCaching.GetFromMemoryCache();
            Console.WriteLine(m);

            var h = await hCaching.GetFromHybridCache();
            Console.WriteLine(h);
        }

        #endregion

        #region Params C#13
        //Params performs implicit conversion of comma separated values to Array
        //issue: performance is bad, because of ref type, function can change the values num[2]= 5;
        //you can't use List<int>, Enumerable, custom etc. ONLY Array allowed.
        //A function can have only one argument of type params and it should be in the end
        //You can now use params with List, IEnumerable, CustomClass implenting IEnumerable that in C#13
        
        int SumNumbersInt(params int[] intArr)
        {
            return intArr.Sum();
        }

        var x = SumNumbersInt(1, 2, 3);

        int SumNumbersSpan(params ReadOnlySpan<int> numbers)
        {
            var sum = 0;
            for (var i = 0; i < numbers.Length; i++)
            {
                sum += numbers[i];
            }
            return sum;
        }

        // also you can use Span, ReadOnlySpan that is much faster.


        Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

        var sum = SumNumbers(1, 2, 3);
        Console.WriteLine(sum);
            stopwatch.Stop();
            Console.WriteLine($"SumNumbers     Value (Elapsed time: {stopwatch.ElapsedTicks:N0} ticks)");
            stopwatch.Reset();

            stopwatch.Start();

        var sum2 = SumNumbersSpan(1, 2, 3);
            stopwatch.Stop();
        Console.WriteLine($"SumNumbersSpan Value (Elapsed time: {stopwatch.ElapsedTicks:N0} ticks)");

        Console.WriteLine(sum2);


        //you can use Enumerable and ReadOnlySpan, Span, List
        int SumNumbers(params MyNumbers numbers)
        {
            return numbers.Sum();
        }



        #endregion

        #region CollectionAccess C#13
        int[] arr = { 1, 2, 3, 4, 5 };


        Console.WriteLine(arr[arr.Length-1]);

        Console.WriteLine(arr[^1]);

        int[] numbers = new int[3];

        numbers[^3] = 10;
        numbers[^2] = 30;
        numbers[^1] = 50;

        Console.WriteLine(numbers[^1]);


        #endregion

        #region Semi-AutoProperties C#13
        /*
         * 
         */
        //https://sharplab.io/#v2:EYLgZgpghgLgrgJwgZwLTIgWwJaqnGAe1QAcFCTkAaAExAGoAfIA

        #endregion
    }
}


