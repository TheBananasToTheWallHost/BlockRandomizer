using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using GeneratorExtensions;

namespace BlockRandomizer {

    /// <summary>
    /// inputs: 
    /// -base_block(string) 
    /// -block_origin(string) 
    /// -base_name_of_saves(string) 
    /// -number_of_blocks_per_trial(int) 
    /// -number_of_trials_to_generate(int) 
    /// -allow_consecutive_equal_values(int)
    /// </summary>
    class TrialGenerator {
        static void Main(string[] args) {
            string contents;
            InputOrigin input_origin;
            //Regex validate_int = new Regex(@"^\d+$");

            while (true) {
                Console.Write("Press Q to quit or any other key to continue: ");
                ConsoleKeyInfo action = Console.ReadKey();

                if (action.Key == ConsoleKey.Q) {
                    break;
                }
                Console.Write(Environment.NewLine);

                Console.Write("Enter your inputs: ");
                string input = Console.ReadLine();
                string[] input_args = input.Split(' ');

                if (input_args.Length != 6) {
                    Console.WriteLine("An error occured: incorrect number of inputs");
                    continue;
                }
                else {

                    try {
                        input_origin = GetPathType(input_args[1]);
                    }
                    catch (ArgumentException e) {
                        Console.WriteLine("An error occured: {0}", e.Message);
                        continue;
                    }

                    try {
                        contents = GetContents(input_args[0], input_origin);
                    }
                    catch (Exception e) {
                        Console.WriteLine("An error occured: {0}", e.Message);
                        continue;
                    }
                }

                int repetitions, num_blocks, allow_consecutive_repeats;

                try {
                    repetitions = Convert.ToInt32(input_args[4]);
                    num_blocks = Convert.ToInt32(input_args[3]);
                    allow_consecutive_repeats = Convert.ToInt32(input_args[5]);
                }
                catch (Exception e) {
                    Console.WriteLine("An error occured: {0}", e.Message);
                    continue;
                }

                string[] space_split_contents = contents.Split(' ');

                Random generator = new Random();


                for (int reps = 0; reps < repetitions; reps++) {
                    try {
                        string[] randomized_contents = GetRandomizedContents(space_split_contents, num_blocks, allow_consecutive_repeats, generator);

                        Regex check_extension = new Regex(@"\.");
                        string save_name = "";

                        Match match = check_extension.Match(input_args[2]);

                        if (match.Success) {
                            int dot_index = match.Index;
                            save_name = input_args[2].IndexSubstring(0, dot_index - 1);
                        }
                        else {
                            save_name = input_args[2];
                        }

                        save_name += (reps + 1);
                        SaveContentsToFile(save_name, randomized_contents);
                    }
                    catch (Exception e) {
                        Console.WriteLine("An error occured: {0}", e.Message);
                        continue;
                    }
                }

                Console.WriteLine("Success!" + Environment.NewLine);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private static string GetContents(string file_info, InputOrigin input_origin) {

            string contents = "";

            switch (input_origin) {
                case InputOrigin.DefaultPath:
                    string desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                    string directory = @"Trial Generator Files\Input_Blocks";
                    string directory_path = Path.Combine(desktop, directory);

                    if (!Directory.Exists(directory_path)) {
                        Directory.CreateDirectory(directory_path);
                    }

                    string full_path = Path.Combine(directory_path, file_info);
                    contents = File.ReadAllText(full_path);
                    break;
                case InputOrigin.CustomPath:
                    contents = File.ReadAllText(file_info);
                    break;
                case InputOrigin.Console:
                    contents = file_info;
                    break;
                default:
                    break;
            }

            return contents;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="flag"></param>
        /// <returns></returns>
        private static InputOrigin GetPathType(string flag) {
            switch (flag) {
                case "-file":
                case "-f":
                    return InputOrigin.DefaultPath;
                case "-path":
                case "-p":
                    return InputOrigin.CustomPath;
                case "-direct_input":
                case "-di":
                case "-d":
                case "-console":
                case "-c":
                    return InputOrigin.Console;
                default:
                    throw new ArgumentException("unrecognized flag used");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contents"></param>
        /// <returns></returns>
        private static string[] GetRandomizedContents(string[] contents, int num_blocks, int allow_consecutive_repeats, Random generator) {
            bool allow_consecutive_equal_values = allow_consecutive_repeats != 0 ? true : false;

            if (num_blocks < 1) {
                throw new ArgumentException("The number of blocks in a trial can't be less than 1");
            }

            int size = contents.Length * num_blocks;
            string[] randomized_contents = new string[size];

            List<int> indices = new List<int>();
            int current_index = 0;

            for (int reps = 0; reps < num_blocks; reps++) {

                for (int i = 0; i < contents.Length; i++) {
                    indices.Add(i);
                }

                int random_position = generator.Next(0, indices.Count);               // get random position into list of indices
                int content_index = indices[random_position];                         // get index into the contents
                randomized_contents[current_index] = contents[content_index];         // set next element of randomized contents to value of contents at the randomized
                indices.RemoveAt(random_position);                                    // remove the index into contents that we've already used

                // check that the last element of previous block isn't same as first element of the
                // next if were not allowing consecutive equal values
                if (!allow_consecutive_equal_values && reps > 0) {
                    if (randomized_contents[current_index] == randomized_contents[current_index - 1]) {
                        int new_index = generator.Next(0, current_index - 1);
                        SwapItems(randomized_contents, current_index - 1, new_index);
                    }
                }

                current_index++;

                while (indices.Count > 0) {

                    random_position = generator.Next(0, indices.Count);
                    content_index = indices[random_position];
                    randomized_contents[current_index] = contents[content_index];
                    indices.RemoveAt(random_position);
                    current_index++;

                }
            }

            return randomized_contents;
        }

        private static void SaveContentsToFile(string filename, string[] contents) {
            

            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            string directory = @"Trial Generator Files\Output_Trials";
            string directory_path = Path.Combine(desktop, directory);

            if (!Directory.Exists(directory_path)) {
                Directory.CreateDirectory(directory_path);
            }

            string filename_with_extension = filename + ".txt";
            Console.WriteLine(filename_with_extension);
            string full_path = Path.Combine(directory_path, filename_with_extension);
            string new_contents = contents.Aggregate((accumulator, next_item) => accumulator + " " + next_item);
            FileStream file = File.Create(full_path);

            file.Write(Encoding.ASCII.GetBytes(new_contents), 0, Encoding.ASCII.GetByteCount(new_contents));
            file.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index1"></param>
        /// <param name="index2"></param>
        private static void SwapItems(string[] array, int index1, int index2) {
            string temp = array[index1];
            array[index1] = array[index2];
            array[index2] = temp;
        }

        private enum InputOrigin {
            DefaultPath,
            CustomPath,
            Console
        }
    }
}
