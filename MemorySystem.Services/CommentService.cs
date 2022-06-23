﻿namespace MemorySystem.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using AutoMapper.QueryableExtensions;
    using MemorySystem.Common.Infrastructure.AutomapperSettings;
    using MemorySystem.Data;
    using MemorySystem.Data.Models;
    using MemorySystem.Services.Models;
    using Microsoft.EntityFrameworkCore;

    public class CommentService : ICommentService
    {
        private readonly MemorySystemDbContext db;

        public CommentService(MemorySystemDbContext db)
        {
            this.db = db;
        }

        public async Task<Result<int>> CreateAsync(CreateCommentModel commentModel, string userId)
        {
            var memory = Mapper.Map<Comment>(commentModel);
            memory.OwnerId = userId;

            this.db.Comments.Add(memory);
            var id = await this.db.SaveChangesAsync();

            return Result<int>.Success(id);
        }

        public async Task<Result<CommentInfoModel>> GetInfo(int id)
        {
            var info = await this.db.Comments.Where(c => c.Id == id).ProjectTo<CommentInfoModel>().FirstOrDefaultAsync();
            if (info == null)
            {
                return Result<CommentInfoModel>.Error("Comment not found");
            }

            return Result<CommentInfoModel>.Success(info);
        }

        public async Task<Result<IEnumerable<CommentInfoModel>>> GetAllCommentsByMemoryId(int memoryId)
            => Result<IEnumerable<CommentInfoModel>>.Success(
                await this.db.Comments.Where(c => c.Id == memoryId).ProjectTo<CommentInfoModel>().ToListAsync());
    }
}
