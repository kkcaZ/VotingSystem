using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc.Formatters.Xml;

namespace VotingSystem.DataAccess.Abstraction;

/// <summary>
/// Abstract data access class containing base CRUD operations
/// Inspiration for CRUD operations taken from: https://www.c-sharpcorner.com/article/crud-operation-using-dapper-in-c-sharp/
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IDataAccess<T>
{
    /// <summary>
    /// Retrieves record from database using <paramref name="id"/>
    /// </summary>
    /// <param name="id">Id of record to be retrieved from database</param>
    /// <returns>Returns record as type <see cref="T"/></returns>
    public abstract T? GetById(Guid id);
    
    /// <summary>
    /// Adds record to the database
    /// </summary>
    /// <param name="record"></param>
    /// <returns>Number of records affected</returns>
    public abstract int Add(T record);
    
    /// <summary>
    /// Attempts to remove record with given id from the database
    /// </summary>
    /// <param name="id">Id of record to be removed from database</param>
    /// <returns>Number of records affected</returns>
    public abstract int Delete(Guid id);

    /// <summary>
    /// Updates record in the database using id attribute from <paramref name="record"/>
    /// </summary>
    /// <param name="record">New record used to update values of record in the database</param>
    /// <returns>Number of records affected</returns>
    public abstract int Update(T record);
}