using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab4.Solvers
{
    // Выбрать 4 метода в соответствии с вариантом
    // Не менять сигнатуры методов, название интерфейса и пространства имен
    // files - массив с txt-файлами (абс. пути)
    // delimiters - массив с разделителями слов (' ', '.' и т.п.)
    // Обработка без учёта регистра!
    public interface ITextFileSolver
    {
        #region V1.1
        // Самое часто встречающееся слово по всем файлам
        string GetTheMostPopularWord(string[] files, char[] delimiters);
        // N часто встречающихся общеупотребимых слов, которые встречаются в каждом файле хотя бы K раз
        IList<string> GetCommonWords(string[] files, char[] delimiters, int N, int K);
        // Файл с наибольшим количеством уникальных слов (слова, которые не повторяются в рамках одного файла) и число таких слов
        ValueTuple<string, int> GetFileWithManyUniqueWords(string[] files, char[] delimiters);
        // Распределение количества слов по длине (по возрастанию)
        IDictionary<int, int> GetWordDistributionByLength(string[] files, char[] delimiters);
        #endregion
    }

    public class LinqSolver : ITextFileSolver
    {
        public IList<string> GetCommonWords(string[] files, char[] delimiters, int N, int K)
        {
            var ArrayWords = files.SelectMany(f => File.ReadAllText(f).ToLower().Split(delimiters)).ToArray();
            var ArrayWordsFile = files.Select(f => File.ReadAllText(f).ToLower().Split(delimiters)).ToArray();
            var DistWords = files.SelectMany(f => File.ReadAllText(f).ToLower().Split(delimiters)).Distinct().ToArray();

            var CountValue = DistWords.Select(w =>
            {
                bool cond = true;
                int count = 0;

                for (int i = 0; i < ArrayWordsFile.Length; i++)
                {
                    count += ArrayWordsFile[i].Where(w2 => w2 == w).ToArray().Length;
                    if (ArrayWordsFile[i].Where(w2 => w2 == w).ToArray().Length < K)
                    {
                        cond = false;
                        break;
                    }
                }
                var s = new
                {
                    word = w,
                    count = count,
                    cond = cond
                };
                return s;
            });

            var resultList = CountValue.Where(s => s.cond == true).OrderByDescending(s => s.count).Take(N).Select(w => w.word).ToList();
            return resultList;
        }

        public (string, int) GetFileWithManyUniqueWords(string[] files, char[] delimiters)
        {
            var ArrayWordsFile = files.Select(f => File.ReadAllText(f).ToLower().Split(delimiters)).ToArray();
            int maxCount = 0;
            string FileName = "default";

            for (int i = 0; i < ArrayWordsFile.Length; i++)
            {
                int count = ArrayWordsFile[i].Distinct().Count();
                if (count > maxCount)
                {
                    maxCount = count;
                    FileName = files[i];
                }
            }

            return (FileName, maxCount);
        }

        public string GetTheMostPopularWord(string[] files, char[] delimiters)
        {
            var ArrayWords = files.SelectMany(f => File.ReadAllText(f).ToLower().Split(delimiters)).ToArray();
            var DistWords = files.SelectMany(f => File.ReadAllText(f).ToLower().Split(delimiters)).Distinct().ToArray();

            var CountValue = DistWords.Select(w =>
            {
                int count = 0;
                ArrayWords.Where(w2 =>
                {
                    if (w2 == w)
                    {
                        count++;
                        return true;
                    }
                    else return false;
                }).ToArray();
                var s = new
                {
                    word = w,
                    count = count
                };
                return s;
            }).ToArray();

            int max = 0;
            if (CountValue.Length != 0)
                max = CountValue.Max(n => n.count);
            var maxWord = CountValue.Where(n => n.count == max).Take(1).ToArray();
            if (maxWord.Length != 0)
                return maxWord[0].word;
            else
                return "";
        }

        public IDictionary<int, int> GetWordDistributionByLength(string[] files, char[] delimiters)
        {
            var ArrayWords = files.SelectMany(f => File.ReadAllText(f).ToLower().Split(delimiters)).ToArray();
            var DistWords = files.SelectMany(f => File.ReadAllText(f).ToLower().Split(delimiters)).Distinct().ToArray();

            var LengthArray = DistWords.Select(w => w.Length).ToArray().Distinct().OrderBy(t => t).Select(l =>
            {
                int count = 0;
                ArrayWords.Where(w =>
                {
                    if (w.Length == l && w.Length != 0)
                    {
                        count++;
                        return true;
                    }
                    return false;
                }).ToArray();
                var s = new
                {
                    length = l,
                    count = count
                };
                return s;
            }).ToArray();

            Dictionary<int, int> res = new Dictionary<int, int>();
            foreach (var item in LengthArray)
            {
                if (item.length != 0)
                    res[item.length] = item.count;
            }
            return res;
        }
    }

    class Subject : IComparable<Subject>
    {
        public string word;
        public int count;
        public bool cond;

        public Subject(string word, int count, bool cond)
        {
            this.word = word;
            this.count = count;
            this.cond = cond;
        }

        public int CompareTo(Subject? other)
        {
            if (other == null) return 1;
            if (other.count > this.count)
            {
                return 1;
            }
            else if (other.count == this.count)
            {
                return 0;
            }
            else
            {
                return -1;
            }
        }
    }

    class SubjectDic
    {
        public int length;
        public int count;

        public SubjectDic(int length, int count)
        {
            this.length = length;
            this.count = count;
        }
    }

    public class NoLinqSolver : ITextFileSolver
    {
        public string[] Dist(string[] arr)
        {
            var newArr = new string[arr.Length];
            int trueLength = 0;

            for (int i = 0; i < arr.Length; i++)
            {
                if (!newArr.Contains(arr[i]))
                {
                    newArr[trueLength++] = arr[i];
                }
            }

            var copy = new string[trueLength];
            for (int i = 0; i < trueLength; i++)
            {
                copy[i] = newArr[i];
            }

            return copy;
        }
        public IList<string> GetCommonWords(string[] files, char[] delimiters, int N, int K)
        {
            string[] ArrayWords = new string[0];
            string[][] ArrayWordsFile = new string[files.Length][];
            for (int i = 0; i < files.Length; i++)
            {
                var f = File.ReadAllText(files[i]).ToLower().Split(delimiters).ToArray();

                var z = new string[ArrayWords.Length + f.Length];
                ArrayWords.CopyTo(z, 0);
                f.CopyTo(z, ArrayWords.Length);


                ArrayWords = new string[ArrayWords.Length + f.Length];
                z.CopyTo(ArrayWords, 0);

                ArrayWordsFile[i] = new string[f.Length];
                f.CopyTo(ArrayWordsFile[i], 0);
            }

            var DistWords = Dist(ArrayWords);

            Subject[] CountValue = new Subject[DistWords.Length];


            for (int i = 0; i < DistWords.Length; i++)
            {
                bool cond = true;
                int count = 0;

                for (int z = 0; z < ArrayWordsFile.Length; z++)
                {
                    int localCount = 0;
                    for (int y = 0; y < ArrayWordsFile[z].Length; y++)
                    {
                        if (ArrayWordsFile[z][y] == DistWords[i])
                        {
                            count++;
                            localCount++;
                        }
                    }
                    if (localCount < K)
                    {
                        cond = false;
                        break;
                    }
                }
                CountValue[i] = new Subject(DistWords[i], count, cond);
            }

            List<string> resultList = new List<string>();
            List<Subject> sortList = new List<Subject>();
            for (int i = 0; i < CountValue.Length; i++)
            {
                if (CountValue[i].cond == true)
                    sortList.Add(CountValue[i]);
            }
            sortList.Sort();
            for (int i = 0; i < N; i++)
            {
                if (i < sortList.Count)
                    resultList.Add(sortList[i].word);
            }
            return resultList;
        }

        public (string, int) GetFileWithManyUniqueWords(string[] files, char[] delimiters)
        {
            string[][] ArrayWordsFile = new string[files.Length][];
            for (int i = 0; i < files.Length; i++)
            {
                var f = File.ReadAllText(files[i]).ToLower().Split(delimiters).ToArray();

                ArrayWordsFile[i] = new string[f.Length];
                f.CopyTo(ArrayWordsFile[i], 0);
            }

            int maxCount = 0;
            string FileName = "default";

            for (int i = 0; i < ArrayWordsFile.Length; i++)
            {
                int count = Dist(ArrayWordsFile[i]).Length;
                if (count > maxCount)
                {
                    maxCount = count;
                    FileName = files[i];
                }
            }

            return (FileName, maxCount);
        }

        public string GetTheMostPopularWord(string[] files, char[] delimiters)
        {
            string[] ArrayWords = new string[0];
            for (int i = 0; i < files.Length; i++)
            {
                var f = File.ReadAllText(files[i]).ToLower().Split(delimiters).ToArray();

                var z = new string[ArrayWords.Length + f.Length];
                ArrayWords.CopyTo(z, 0);
                f.CopyTo(z, ArrayWords.Length);


                ArrayWords = new string[ArrayWords.Length + f.Length];
                z.CopyTo(ArrayWords, 0);

            }

            var DistWords = Dist(ArrayWords);

            Dictionary<string, int> wordDict = new Dictionary<string, int>();
            for (int i = 0; i < DistWords.Length; i++)
            {
                wordDict[DistWords[i]] = 0;
            }

            for (int i = 0; i < ArrayWords.Length; i++)
            {
                wordDict[ArrayWords[i]]++;
            }

            string maxWord = "";
            int max = 0;

            foreach (var item in wordDict)
            {
                if (item.Value > max)
                {
                    max = item.Value;
                    maxWord = item.Key;
                }
            }



            return maxWord;
        }

        public IDictionary<int, int> GetWordDistributionByLength(string[] files, char[] delimiters)
        {
            string[] ArrayWords = new string[0];
            for (int i = 0; i < files.Length; i++)
            {
                var f = File.ReadAllText(files[i]).ToLower().Split(delimiters).ToArray();

                var z = new string[ArrayWords.Length + f.Length];
                ArrayWords.CopyTo(z, 0);
                f.CopyTo(z, ArrayWords.Length);


                ArrayWords = new string[ArrayWords.Length + f.Length];
                z.CopyTo(ArrayWords, 0);

            }

            var DistWords = Dist(ArrayWords);

            List<int> LenList = new List<int>();
            for (int i = 0; i < DistWords.Length; i++)
            {
                if (!LenList.Contains(DistWords[i].Length))
                {
                    LenList.Add(DistWords[i].Length);
                }
            }

            LenList.Sort();

            SubjectDic[] LengthArray = new SubjectDic[LenList.Count];

            for (int i = 0; i < LenList.Count; i++)
            {
                int count = 0;

                for (int j = 0; j < ArrayWords.Length; j++)
                {
                    if (ArrayWords[j].Length == LenList[i] && ArrayWords[j].Length != 0)
                    {
                        count++;
                    }
                }
                LengthArray[i] = new SubjectDic(LenList[i], count);
            }


            Dictionary<int, int> res = new Dictionary<int, int>();
            foreach (var item in LengthArray)
            {
                if (item.length != 0)
                    res[item.length] = item.count;
            }
            return res;
        }
    }
}
