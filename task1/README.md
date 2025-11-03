# Task 1 - Basic C# Methods

Simple console application in C# that implements four small, efficient helper methods and prints sample output. The goal is to demonstrate core language features such as bitwise operations, string manipulation, and loop control.

---

## Overview
- Bitwise: detect whether a number is a power of two.
- Strings: reverse text and replicate text.
- Iteration: print odd numbers between 1 and 99.
- All code lives in `task1.cs` under the `Program` class and runs via `Task1.csproj`.

---

## Methods

### 1. `IsPowerOfTwo(long value)`
Uses the classic `(value & (value - 1)) == 0` trick to ensure only a single bit is set.

**Example**
```
IsPowerOfTwo(8)  -> True
IsPowerOfTwo(9)  -> False
```

### 2. `Reverse(string text)`
Returns a reversed copy of the input string by using `Array.Reverse`.

**Example**
```
Reverse("Hello") -> "olleH"
```

### 3. `Replicate(string text, int count)`
Builds a new string by repeating `text` `count` times with `StringBuilder`.

**Example**
```
Replicate("Hi", 3) -> "HiHiHi"
```

### 4. `PrintOddNumbers()`
Writes odd numbers from 1 through 99 to the console by incrementing the loop counter by 2.

---

## Sample Output
```
True
False
olleH
HiHiHi
1
3
5
...
99
```

---

## Run the Program
```bash
cd task1
dotnet run
```

---

## Files
```
task1/
- task1.cs
- Task1.csproj
- README.md
```
