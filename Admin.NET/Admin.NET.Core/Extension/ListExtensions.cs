// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

public static class ListExtensions
{
    public static async Task ForEachAsync<T>(this List<T> list, Func<T, Task> func)
    {
        foreach (var value in list)
        {
            await func(value);
        }
    }

    public static async Task ForEachAsync<T>(this IEnumerable<T> source, Func<T, Task> action)
    {
        foreach (var value in source)
        {
            await action(value);
        }
    }

    public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> consumer)
    {
        foreach (T item in enumerable)
        {
            consumer(item);
        }
    }

    public static void AddRange<T>(this IList<T> list, IEnumerable<T> items)
    {
        if (list is List<T> list2)
        {
            list2.AddRange(items);
            return;
        }

        foreach (T item in items)
        {
            list.Add(item);
        }
    }
}