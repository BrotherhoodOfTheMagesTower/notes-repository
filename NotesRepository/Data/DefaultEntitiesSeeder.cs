using Microsoft.AspNetCore.Identity;
using NotesRepository.Areas.Identity.Data;
using NotesRepository.Data.Models;
using NotesRepository.Repositories;
using NotesRepository.Services;
using NotesRepository.Services.Azure;
using Directory = NotesRepository.Data.Models.Directory;

namespace NotesRepository.Data
{
    public static class DefaultEntitiesSeeder
    {
        /// <summary>
        /// This method is responsible for seeding default directories, notes & events for one particular user - admin@admin.com
        /// </summary>
        /// <param name="serviceProvider"></param>
        public static async void SeedDefaultEntities(this IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetService<ApplicationDbContext>();
            var ns = new NoteService(new NoteRepository(context), new UserRepository(context), new EventRepository(context), new DirectoryRepository(context), new ImageRepository(context));

            // Admin data
            var user = new ApplicationUser
            {
                Id = "ba6c78a4-9cff-4bb1-acbd-f4c23a063616",
                Email = "admin@admin.com",
                NormalizedEmail = "ADMIN@ADMIN.COM",
                UserName = "admin@admin.com",
                NormalizedUserName = "ADMIN@ADMIN.COM",
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            // default directory data
            var defDir = new Directory
            {
                DirectoryId = Guid.Parse("5357fe5b-e0c5-40a3-98be-6776844dbaf7"),
                Name = "Default",
                User = user
            };

            // bin directory data
            var defBinDir = new Directory
            {
                DirectoryId = Guid.Parse("e7ce7d86-314c-4ab6-a33e-4c6349b5a1cc"),
                Name = "Bin",
                User = user
            };

            // subdirectories for default directory data
            var subDirsOfDefault = new List<Directory>
            {
                new Directory { DirectoryId = Guid.Parse("1125c320-d885-4d32-a8cb-2f11fe1b64ef"), Name = "SubDirOfDefault_1", ParentDir = defDir, User = user },
                new Directory
                {
                    DirectoryId = Guid.Parse("8e069728-754f-4580-8426-9d6f160ea952"),
                    Name = "SubDirOfDefault_2",
                    ParentDir = defDir,
                    User = user,
                    SubDirectories = new List<Directory> { new Directory { DirectoryId = Guid.Parse("a3c77e40-6b71-4776-ba00-08f128be6018"), Name = "SubSubDirOfDefault2_1", User = user },
                        new Directory { DirectoryId = Guid.Parse("f8622d56-a8b9-4132-8a7c-133adbbece18"), Name = "SubSubDirOfDefault2_2", User = user } }
                },
            };

            // custom directory with subdirectories data
            var customDirWithSubDirs = new Directory
            {
                DirectoryId = Guid.Parse("ec3720dd-8d33-406d-b3f5-cdc70d5a9955"),
                Name = "CustomDir",
                User = user,
                SubDirectories = new List<Directory> { new Directory { DirectoryId = Guid.Parse("6a5931d2-8457-4804-bf44-b6548f00ebdf"), Name = "SubDirOfCustom_1", User = user },
                    new Directory { DirectoryId = Guid.Parse("9833eefb-b902-4d4e-93d8-bc760325e576"), Name = "SubDirOfCustom_2", User = user } }
            };

            if (!context.Users.Any(x => x.Id == user.Id))
            {
                user.HashPassword("Admin123!");
                context.Users.Add(user);
                context.SaveChanges();
            }

            #region seedDirectories


            if (!context.Directories.Any(x => x.DirectoryId == defDir.DirectoryId))
            {
                context.Directories.Add(defDir);
                context.SaveChanges();
            }

            if (!context.Directories.Any(x => x.DirectoryId == defBinDir.DirectoryId))
            {
                context.Directories.Add(defBinDir);
                context.SaveChanges();
            }

            if (!context.Directories.Any(x => x.DirectoryId == customDirWithSubDirs.DirectoryId))
            {
                context.Directories.Add(customDirWithSubDirs);
                context.SaveChanges();
            }

            if (!context.Directories.Any(x => x.DirectoryId == subDirsOfDefault.ElementAt(0).DirectoryId) &&
               !context.Directories.Any(x => x.DirectoryId == subDirsOfDefault.ElementAt(1).DirectoryId) &&
               !context.Directories.Any(x => x.DirectoryId == subDirsOfDefault.ElementAt(1).SubDirectories!.ElementAt(0).DirectoryId) &&
               !context.Directories.Any(x => x.DirectoryId == subDirsOfDefault.ElementAt(1).SubDirectories!.ElementAt(1).DirectoryId))
            {
                context.Directories.AddRange(subDirsOfDefault);
                context.SaveChanges();
            }
            #endregion

            #region seedNotes
            if (!context.Notes.Any(x => x.NoteId == Guid.Parse("9c755080-ba6b-48f1-854d-1816ad5fa74a")))
            {
                var note = new Note
                {
                    NoteId = Guid.Parse("9c755080-ba6b-48f1-854d-1816ad5fa74a"),
                    Title = "Password list",
                    Content = "Note under default directory",
                    CreatedAt = new DateTime(2019, 12, 24, 4, 20, 0),
                    EditedAt = new DateTime(2021, 4, 21, 7, 42, 0),
                    EditedBy = user,
                    Owner = user,
                    Directory = defDir
                };
                context.Notes.Add(note);
                context.SaveChanges();
            }

            if (!context.Notes.Any(x => x.NoteId == Guid.Parse("0b2f78d3-d292-462e-a889-dd5c541f131b")))
            {
                var note = new Note
                {
                    NoteId = Guid.Parse("0b2f78d3-d292-462e-a889-dd5c541f131b"),
                    Title = "Trips I want to take",
                    Content = "Note under custom directory",
                    CreatedAt = new DateTime(2021, 4, 12, 5, 12, 0),
                    EditedAt = new DateTime(2021, 4, 12, 5, 12, 0),
                    EditedBy = user,
                    Owner = user,
                    Directory = customDirWithSubDirs
                };
                context.Notes.Add(note);
                context.SaveChanges();
            }

            if (!context.Notes.Any(x => x.NoteId == Guid.Parse("9bb2e297-6a70-4517-82b5-7fca43550c3e")))
            {
                var note = new Note
                {
                    NoteId = Guid.Parse("9bb2e297-6a70-4517-82b5-7fca43550c3e"),
                    Title = "Sicilia monuments",
                    Content = "Note under subdirectory_1 of custom dir",
                    CreatedAt = new DateTime(2018, 6, 1, 2, 2, 0),
                    EditedAt = new DateTime(2018, 6, 1, 2, 2, 0),
                    EditedBy = user,
                    Owner = user,
                    Directory = customDirWithSubDirs.SubDirectories.ElementAt(0)
                };
                context.Notes.Add(note);
                context.SaveChanges();
            }

            if (!context.Notes.Any(x => x.NoteId == Guid.Parse("8e37c761-b4af-40bd-b2aa-f260a9b5d5b7")))
            {
                var note = new Note
                {
                    NoteId = Guid.Parse("8e37c761-b4af-40bd-b2aa-f260a9b5d5b7"),
                    Title = "Madeira monuments",
                    Content = "Note under subdirectory_2 of custom dir",
                    CreatedAt = new DateTime(2019, 9, 5, 12, 44, 0),
                    EditedAt = new DateTime(2022, 2, 12, 17, 21, 0),
                    EditedBy = user,
                    Owner = user,
                    Directory = customDirWithSubDirs.SubDirectories.ElementAt(1)
                };
                context.Notes.Add(note);
                context.SaveChanges();
            }

            if (!context.Notes.Any(x => x.NoteId == Guid.Parse("8d44780e-592d-4003-a271-69c186653dda")))
            {
                var note = new Note
                {
                    NoteId = Guid.Parse("8d44780e-592d-4003-a271-69c186653dda"),
                    Title = "Fast cars",
                    Content = "Note under subdirectory_2 of default dir",
                    CreatedAt = new DateTime(2019, 9, 5, 12, 44, 0),
                    EditedAt = new DateTime(2019, 9, 5, 12, 44, 0),
                    EditedBy = user,
                    Owner = user,
                    Directory = subDirsOfDefault.ElementAt(1)
                };
                context.Notes.Add(note);
                context.SaveChanges();
            }

            if (!context.Notes.Any(x => x.NoteId == Guid.Parse("1e3a6c05-18d5-4b16-a546-b28f243e0f72")))
            {
                var note = new Note
                {
                    NoteId = Guid.Parse("1e3a6c05-18d5-4b16-a546-b28f243e0f72"),
                    Title = "BMW M4",
                    Content = "Note under subsubdirectory_1 of default dir",
                    CreatedAt = new DateTime(2022, 2, 1, 5, 23, 0),
                    EditedAt = new DateTime(2022, 2, 1, 5, 23, 0),
                    EditedBy = user,
                    Owner = user,
                    Directory = subDirsOfDefault.ElementAt(1).SubDirectories.ElementAt(0)
                };
                context.Notes.Add(note);
                context.SaveChanges();
            }

            if (!context.Notes.Any(x => x.NoteId == Guid.Parse("a6fd2e45-df97-4de1-8119-76a6191fb690")))
            {
                var note = new Note
                {
                    NoteId = Guid.Parse("a6fd2e45-df97-4de1-8119-76a6191fb690"),
                    Title = "Audi RS6",
                    Content = "Note under subsubdirectory_2 of default dir",
                    CreatedAt = new DateTime(2017, 11, 12, 2, 11, 0),
                    EditedAt = new DateTime(2020, 9, 3, 15, 12, 0),
                    EditedBy = user,
                    Owner = user,
                    Directory = subDirsOfDefault.ElementAt(1).SubDirectories.ElementAt(1)
                };
                context.Notes.Add(note);
                context.SaveChanges();
            }

            if (!context.Notes.Any(x => x.NoteId == Guid.Parse("c32c63ec-308c-4051-82fb-1a62548b333a")))
            {
                var note = new Note
                {
                    NoteId = Guid.Parse("c32c63ec-308c-4051-82fb-1a62548b333a"),
                    Title = "Audi SQ7",
                    Content = "Note under subsubdirectory_2 of default dir",
                    CreatedAt = new DateTime(2019, 2, 12, 9, 43, 0),
                    EditedAt = new DateTime(2019, 2, 12, 9, 43, 0),
                    EditedBy = user,
                    Owner = user,
                    Directory = subDirsOfDefault.ElementAt(1).SubDirectories.ElementAt(1)
                };
                context.Notes.Add(note);
                context.SaveChanges();
            }
            
            if (!context.Notes.Any(x => x.NoteId == Guid.Parse("426c6d6c-8c0a-4b34-933c-7df94a205127")))
            {
                var note = new Note
                {
                    NoteId = Guid.Parse("426c6d6c-8c0a-4b34-933c-7df94a205127"),
                    Title = "Gift ideas for aunt's birthday",
                    Content = "Samsung Galaxy S20 // Oppo Reno 5",
                    CreatedAt = new DateTime(2022, 2, 14, 9, 43, 0),
                    EditedAt = new DateTime(2022, 2, 14, 9, 43, 0),
                    EditedBy = user,
                    Owner = user,
                    Directory = defDir
                };
                context.Notes.Add(note);
                context.SaveChanges();
            }
            if (!context.Notes.Any(x => x.NoteId == Guid.Parse("c258aff3-5e0a-4a9f-8bba-15dce9b27a0a")))
            {
                var note = new Note
                {
                    NoteId = Guid.Parse("c258aff3-5e0a-4a9f-8bba-15dce9b27a0a"),
                    Title = "From start in the bin.",
                    Content = "This note was seeded, being already in recycle bin.",
                    CreatedAt = new DateTime(2018, 2, 14, 9, 43, 0),
                    EditedAt = new DateTime(2018, 2, 14, 9, 43, 0),
                    EditedBy = user,
                    DeletedAt = DateTime.Now,
                    IsMarkedAsDeleted = true,
                    Owner = user,
                    Directory = defBinDir
                };
                context.Notes.Add(note);
                context.SaveChanges();
            }
            #endregion

            #region seedEvents
            var ur = new UserRepository(context);
            var adminUser = await ur.GetUserByIdAsync(user.Id);
            var noteForEvent = await ns.GetNoteByIdAsync(Guid.Parse("426c6d6c-8c0a-4b34-933c-7df94a205127"));
            if (adminUser is not null && noteForEvent is not null)
            {
                //seed 4 events with(out) reminder & with(out) attached note
                var events = new List<Event>
                {
                    new Event(Guid.Parse("64fb58cd-a344-4dfb-88e0-594e3cabecec"), "Mommy's 50th birthday", new DateTime(2022, 5, 28), new DateTime(2022, 5, 28), adminUser, new DateTime(2022, 5, 27, 8, 0, 0)),
                    new Event(Guid.Parse("a9436543-5db5-4756-b8b7-07854d1678d0"), "Cinema with Emily", new DateTime(2022, 6, 12), new DateTime(2022, 6, 12), adminUser),
                    new Event(Guid.Parse("78e16732-1e76-4a3e-a830-3aadd62e99fd"), "Trip to Berlin", new DateTime(2022, 6, 17), new DateTime(2022, 6, 22), adminUser),
                    new Event(Guid.Parse("d0deef49-e9b1-4c2c-b248-218642f44504"), "Buy gift for aunt", new DateTime(2022, 6, 19), new DateTime(2022, 6, 19), adminUser, new DateTime(2022, 6, 18), noteForEvent)
                };
                if (!context.Events.Any(x => x.EventId == events.ElementAt(0).EventId))
                {
                    context.Events.Add(events.ElementAt(0));
                    context.SaveChanges();
                }

                if (!context.Events.Any(x => x.EventId == events.ElementAt(1).EventId))
                {
                    context.Events.Add(events.ElementAt(1));
                    context.SaveChanges();
                }

                if (!context.Events.Any(x => x.EventId == events.ElementAt(2).EventId))
                {
                    context.Events.Add(events.ElementAt(2));
                    context.SaveChanges();
                }

                if (!context.Events.Any(x => x.EventId == events.ElementAt(3).EventId))
                {
                    context.Events.Add(events.ElementAt(3));
                    context.SaveChanges();
                }
            }

            #endregion
        }

        /// <summary>
        /// This method is responsible for seeding 2 collaborators, which have 2 shared notes with user admin@admin.com
        /// </summary>
        /// <param name="serviceProvider"></param>
        public static void SeedCollaboratorsWithSharedNotes(this IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetService<ApplicationDbContext>();

            // user1 data
            var user1 = new ApplicationUser
            {
                Id = "66657710-2688-4bca-b5d7-c07752d53460",
                Email = "friend@1.com",
                NormalizedEmail = "FRIEND@1.COM",
                UserName = "friend@1.com",
                NormalizedUserName = "FRIEND@1.COM",
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            // default directory data
            var defDir1 = new Directory
            {
                DirectoryId = Guid.Parse("f9a777b3-be99-4883-b8aa-70cc3d8fe70c"),
                Name = "Default",
                User = user1
            };

            // bin directory data
            var defBinDir1 = new Directory
            {
                DirectoryId = Guid.Parse("9ef5ccaf-7b38-4dbe-baf5-86b4ed50e61f"),
                Name = "Bin",
                User = user1
            };

            user1.Directories = new[] { defDir1, defBinDir1 };
            Task.Delay(1000).Wait();

            if (!context.Users.Any(x => x.Id == user1.Id))
            {
                user1.HashPassword("Admin123!");
                context.Users.Add(user1);
                context.SaveChanges();
            }

            if(!context.CollaboratorsNotes.Any(x => x.ApplicationUserId == user1.Id && x.NoteId == Guid.Parse("9c755080-ba6b-48f1-854d-1816ad5fa74a")))
            {
                context.CollaboratorsNotes.Add(new CollaboratorsNotes
                {
                    ApplicationUserId = user1.Id,
                    NoteId = Guid.Parse("9c755080-ba6b-48f1-854d-1816ad5fa74a")
                });
            }

            if (!context.CollaboratorsNotes.Any(x => x.ApplicationUserId == user1.Id && x.NoteId == Guid.Parse("9bb2e297-6a70-4517-82b5-7fca43550c3e")))
            {
                context.CollaboratorsNotes.Add(new CollaboratorsNotes
                {
                    ApplicationUserId = user1.Id,
                    NoteId = Guid.Parse("9bb2e297-6a70-4517-82b5-7fca43550c3e")
                });
            }
            
            // user2 data
            var user2 = new ApplicationUser
            {
                Id = "06154886-0051-4167-b6c1-ffeb7e0bf974",
                Email = "friend@2.com",
                NormalizedEmail = "FRIEND@2.COM",
                UserName = "friend@2.com",
                NormalizedUserName = "FRIEND@2.COM",
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            // default directory data
            var defDir2 = new Directory
            {
                DirectoryId = Guid.Parse("e92bb8f8-ab1f-4813-9d00-73fca5187044"),
                Name = "Default",
                User = user2
            };

            // bin directory data
            var defBinDir2 = new Directory
            {
                DirectoryId = Guid.Parse("853db056-25de-40a1-bd45-8cb3f824a7f0"),
                Name = "Bin",
                User = user2
            };

            user2.Directories = new[] { defDir2, defBinDir2 };
            Task.Delay(1000).Wait();

            if (!context.Users.Any(x => x.Id == user2.Id))
            {
                user1.HashPassword("Admin123!");
                context.Users.Add(user2);
                context.SaveChanges();
            }

            if (!context.CollaboratorsNotes.Any(x => x.ApplicationUserId == user2.Id && x.NoteId == Guid.Parse("8d44780e-592d-4003-a271-69c186653dda")))
            {
                context.CollaboratorsNotes.Add(new CollaboratorsNotes
                {
                    ApplicationUserId = user2.Id,
                    NoteId = Guid.Parse("8d44780e-592d-4003-a271-69c186653dda")
                });
                context.SaveChanges();
            }

            if (!context.CollaboratorsNotes.Any(x => x.ApplicationUserId == user2.Id && x.NoteId == Guid.Parse("c32c63ec-308c-4051-82fb-1a62548b333a")))
            {
                context.CollaboratorsNotes.Add(new CollaboratorsNotes
                {
                    ApplicationUserId = user2.Id,
                    NoteId = Guid.Parse("c32c63ec-308c-4051-82fb-1a62548b333a")
                });
                context.SaveChanges();
            }
                
        }

        private static void HashPassword(this ApplicationUser user, string psswd = "Password123!")
        {
            var password = new PasswordHasher<ApplicationUser>();
            var hashed = password.HashPassword(user, psswd);
            user.PasswordHash = hashed;
        }
    }
}
