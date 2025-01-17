﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using GeneratorExtensions;
using Utilities;

namespace BlockRandomizer {

    /// <summary>
    /// inputs: 
    /// -block(string) 
    /// -block_origin_type(string) 
    /// -base_name_of_saves(string) 
    /// -number_of_blocks_per_file(int) 
    /// -number_of_files(int) 
    /// -allow_consecutive_equal_values(int)
    /// 
    /// This program generates a number of files with randomized block contents repeated a certain number of times. Files are saved at
    /// (Desktop: Trial Generator Files\Output_Trials) as base_name_of_saves plus a number. The block can be input directly in the
    /// console, or be passed in in a file. The file can located anywhere or in the default location (Desktop: Trial Generator Files\Input_Blocks); if
    /// it's located in the default location, only the name of the file needs to be passed in. This program assumes a block only contains unique values
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

                Console.Write("Enter your inputs (block/block path) (block_origin) (base name of saves) (number of block repetitions per file) (number of files) (allow consecutive equal values): ");
                string input = Console.ReadLine();

                string[] input_args = new string[6];

                // Extract arguments from the string. An argument is anything enclosed by quotes 
                // or separated by spaces but not enclosed by quotes
                try {
                    int i = 0;
                    while (!string.IsNullOrEmpty(input)) {
                        
                        input = input.Trim();
                        // argument enclosed by quotes
                        if (input[0] == '\"') {
                            int arg_start = input.IndexOf('\"');
                            int arg_end = input.NextIndexOf('\"', arg_start + 1);
                            string arg = input.IndexSubstring(arg_start, arg_end);
                            arg = arg.Trim('\"');
                            input = input.IndexRemove(arg_start, arg_end);
                            input_args[i] = arg;
                            i++;
                        }
                        else {
                            int arg_end = input.IndexOf(' ');
                            string arg = "";

                            if (arg_end == -1) {
                                arg = input;
                                input = input.Remove(0);
                            }
                            else {
                                arg = input.IndexSubstring(0, arg_end);
                                input = input.IndexRemove(0, arg_end);
                            }
                            arg = arg.Trim();
                            input_args[i] = arg;
                            i++;
                        }
                    }
                }
                catch (Exception e) {
                    Console.WriteLine("An error occured while parsing arguments: {0}", e.Message);
                    continue;
                }



                for (int i = 0; i < input_args.Length; i++) {
                    if (input_args[i] == null) {
                        Console.WriteLine("An error occured: incorrect number of inputs");
                        continue;
                    }
                }

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


                int num_files, num_blocks, allow_consecutive_repeats;

                try {
                    num_files = Convert.ToInt32(input_args[4]);
                    num_blocks = Convert.ToInt32(input_args[3]);
                    allow_consecutive_repeats = Convert.ToInt32(input_args[5]);
                }
                catch (Exception e) {
                    Console.WriteLine("An error occured: {0}", e.Message);
                    continue;
                }

                string[] space_split_contents = contents.Split(' ');

                Random generator = new Random();

                // create files with the given block specifications
                for (int reps = 0; reps < num_files; reps++) {
                    try {
                        //create file contents
                        string[] randomized_contents = GetRandomizedContents(space_split_contents, num_blocks, allow_consecutive_repeats, generator);

                        // check that the desired save name doesn't already contain an extension
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

                        // save file
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
        /// Returns the block contents from a file or the console
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
        /// Takes a flag and returns an InputOrigin indicating
        /// where the block values are going to come from.
        /// </summary>
        /// <param name="flag">a string containing an input origin flag</param>
        /// <returns>InputOrigin indicating where the block values are going to come from</returns>
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
        /// Creates an array of randomized block values. These values are only randomized within each block repetition, not across block repetitions.
        /// Randomized blocks contain all of the values found in the original block.
        /// </summary>
        /// <param name="contents">Array containing the block values</param>
        /// <param name="num_blocks">The number of times the block will be repeated</param>
        /// <param name="allow_consecutive_repeats">Allow values to be repeated. Zero is false, anything else is true</param>
        /// <param name="generator">a Random object</param>
        /// <returns>An array of randomized block values</returns>
        private static string[] GetRandomizedContents(string[] contents, int num_blocks, int allow_consecutive_repeats, Random generator) {
            bool allow_consecutive_equal_values = allow_consecutive_repeats != 0 ? true : false;

            if (num_blocks < 1) {
                throw new ArgumentException("The number of blocks in a trial can't be less than 1");
            }

            //total number of items in the file
            int size = contents.Length * num_blocks;
            string[] randomized_contents = new string[size];

            List<int> indices = new List<int>();
            int current_index = 0;

            for (int reps = 0; reps < num_blocks; reps++) {

                // reset list of indices after pass through due to deletions
                for (int i = 0; i < contents.Length; i++) {
                    indices.Add(i);
                }

                int random_position = generator.Next(0, indices.Count);               // get random position into list of indices
                int content_index = indices[random_position];                         // get index into the contents
                randomized_contents[current_index] = contents[content_index];         // set next element of randomized contents to value of contents at the randomized
                indices.RemoveAt(random_position);                                    // remove the index into contents that we've already used

                // check that the last element of previous block isn't same as first element of the
                // next if we're not allowing consecutive equal values
                if (!allow_consecutive_equal_values && reps > 1) {
                    if (randomized_contents[current_index] == randomized_contents[current_index - 1]) {
                        int new_index = generator.Next(0, current_index - 1);
                        Utils.SwapItems(randomized_contents, current_index - 1, new_index);
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

        /// <summary>
        /// Saves a file and its contents to the programs default location (Desktop: Trial Generator Files\Output_Trials)
        /// </summary>
        /// <param name="filename">the name of the file</param>
        /// <param name="contents">the contents of the file</param>
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
        /// defines possible block origins
        /// </summary>
        private enum InputOrigin {
            DefaultPath,
            CustomPath,
            Console
        }
    }
}
