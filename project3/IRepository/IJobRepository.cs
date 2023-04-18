﻿using project3.Models;

namespace project3.IRepository
{
    public interface IJobRepository
    {
        IEnumerable<job> GetAll(string SortProperty ,SortOrder sortOrder);
        IEnumerable<job> GetAll();
        job GetById(int id);
        job GetByCompanyId(int CiD);

        IEnumerable<job> SearchByName(string name);

        IEnumerable<job> SearchByCategory(int category);

        

        int add_job(string addjob);
        //Task<IEnumerable<user>> GetJobByCity(string city);

        bool Add(job job);
        bool Update(job job);
        bool Delete(job job);
        bool Save();
    }
}
