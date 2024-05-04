int GetMagicSum0(IEnumerable<string> items)
{
    List<int> all = new List<int>();
    using (IEnumerator<string> enumerator = items.GetEnumerator())
    {
        while (enumerator.MoveNext())
        {
            int number;
            if (int.TryParse(enumerator.Current, out number)) all.Add(number);
        }
    }

    if (all.Count < 2) return -1;

    // find the sum of the two largest numbers
    int[] greatestTwo = { all[0], all[1] };
    if (greatestTwo[1] > greatestTwo[0])
    {
        greatestTwo[0] = all[1];
        greatestTwo[1] = all[0];
    }

    for (int i = 2; i < all.Count; i++)
    {
        if (all[i] > greatestTwo[0])
        {
            greatestTwo[1] = greatestTwo[0];
            greatestTwo[0] = all[i];
        }
        else if (all[i] > greatestTwo[1])
        {
            greatestTwo[1] = all[i];
        }
    }

    return greatestTwo[0] + greatestTwo[1];
}

int GetMagicSum1(IEnumerable<string> items)
{
    // #1 Target-typed new expression (C# 9)
    List<int> all = new() { Capacity = 100 }; // #2 Object initializer (C# 3)

    // #3 using declarations (C# 8)
    using IEnumerator<string> enumerator = items.GetEnumerator();

    // #4 foreach loop (C# 1) -> iterator + disposable patterns
    foreach (string item in items)
    {
        // #5 inline out variables (C# 7)
        if (int.TryParse(item, out int number)) all.Add(number);
    }

    if (all.Count < 2) return -1;

    // #6 Tuples (C# 7)
    // #7 Deconstruction (C# 7)
    // #8 Imlicitly typed local variables (C# 3)
    var (a, b) = all[0] >= all[1] ? (all[0], all[1]) : (all[1], all[0]);

    // #9 LINQ (C# 3 / .NET 3.5)
    foreach (int number in all.Skip(2))
    {
        if (number > a) (a, b) = (number, a);
        else if (number > b) b = number;

    }

    return a + b;
}

// Modern C# + Modern C# Thinking
int GetMagicSum2(IEnumerable<string> items)
{
    // Every loop maintains a state.
    // The Loop is a finite state machine, the state of which changes from interation
    // to interation until it comes to an end.

    var (a, b, count) = (0, 0, 0);

    foreach (string item in items)
    {
        // #10 continue/break keywords (C# 1)
        if (!int.TryParse(item, out int number)) continue;
        // #11 switch expressions (C# 8)
        // #12 Pattern mathcing (C# 7)

        // Switch statement represents the process of advancing the state machine under
        // the effect of an input number
        (a, b, count) = count switch
        {
            0 => (number, 0, 1),
            1 when number > a => (number, a, 2),
            1 => (a, number, 2),
            2 when number > a => (number, a, 2),
            2 when number > b => (a, number, 2),
            _ => (a, b, count),
        };
    }

    return count == 2 ? a + b : -1;
}

// #16 Generics (C# 2)
int GetMagicSum3(IEnumerable<string> items)
    // #17 Method gorups as delegates (C# 2)
    => items.Aggregate((max: 0, next: 0, count: 0), AdvanceRaw, ProduceSum);

int ProduceSum((int max, int next, int count) tuple)
    => tuple.count == 2 ? tuple.max + tuple.next : -1;

// Pure function
// #15 Expression-bodied members (C# 7)
(int max, int next, int count) AdvanceRaw((int max, int next, int count) tuple, string item)
    => int.TryParse(item, out int number) ? Advance(tuple, number) : tuple;

// Pure function 
(int max, int next, int count) Advance((int max, int next, int count) tuple, int number)
    => tuple switch
    {
        // #13 Tuple patterns (C# 8)
        // #14 Discard patterns (C# 8)
        (_, _, 0) => (number, 0, 1),
        (var max, _, 1) when number > max => (number, max, 2),
        (var max, _, 1) => (max, number, 2),
        (var max, _, 2) when number > max => (number, max, 2),
        (var max, var next, 2) when number > next => (max, number, 2),
        _ => tuple,
    };

// The GetMagicSum3 method represents how C# works today.
// Pure functions, the way F# is written.
// All branching done via pattern matching.
// All looping done via LINQ.
// All behavior done via expressions.