﻿using System;
using Microsoft.AspNet.Mvc;
using WebVella.ERP.Api.Models;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Linq;
using WebVella.ERP.Api;
using WebVella.ERP.Storage;
using System.Collections.Generic;


// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace WebVella.ERP.Web.Controllers
{
    public class ApiController : ApiControllerBase
    {

        //TODO - add created_by and modified_by fields where needed, when the login is done
        RecordManager recMan;

        public IStorageService Storage { get; set; }
        public ApiController(IErpService service, IStorageService storage) : base(service)
        {
            Storage = storage;
            recMan = new RecordManager(service);
        }
        

        #region << Entity Meta >>
        // Get all entity definitions
        // GET: api/v1/en_US/meta/entity/list/
        [AcceptVerbs(new[] { "GET" }, Route = "api/v1/en_US/meta/entity/list")]
        public IActionResult GetEntityMetaList()
        {
            return DoResponse(new EntityManager(service.StorageService).ReadEntities());
        }

        // Get entity meta
        // GET: api/v1/en_US/meta/entity/{name}/
        [AcceptVerbs(new[] { "GET" }, Route = "api/v1/en_US/meta/entity/{Name}")]
        public IActionResult GetEntityMeta(string Name)
        {
            return DoResponse(new EntityManager(service.StorageService).ReadEntity(Name));
        }


        // Create an entity
        // POST: api/v1/en_US/meta/entity
        [AcceptVerbs(new[] { "POST" }, Route = "api/v1/en_US/meta/entity")]
        public IActionResult CreateEntity([FromBody]InputEntity submitObj)
        {
            return DoResponse(new EntityManager(service.StorageService).CreateEntity(submitObj));
        }

        // Create an entity
        // POST: api/v1/en_US/meta/entity
        [AcceptVerbs(new[] { "PATCH" }, Route = "api/v1/en_US/meta/entity/{StringId}")]
        public IActionResult PatchEntity(string StringId, [FromBody]JObject submitObj)
        {
            FieldResponse response = new FieldResponse();

            Guid entityId;
            if (!Guid.TryParse(StringId, out entityId))
            {
                response.Errors.Add(new ErrorModel("id", StringId, "id parameter is not valid Guid value"));
                return DoResponse(response);
            }

            InputEntity inputEntity = new InputEntity();

            Type inputEntityType = inputEntity.GetType();

            foreach (var prop in submitObj.Properties())
            {
                int count = inputEntityType.GetProperties().Where(n => n.Name.ToLower() == prop.Name.ToLower()).Count();
                if (count < 1)
                    response.Errors.Add(new ErrorModel(prop.Name, prop.Value.ToString(), "Input object contains property that is not part of the object model."));
            }

            if (response.Errors.Count > 0)
                return DoBadRequestResponse(response);

            try
            {
                inputEntity = submitObj.ToObject<InputEntity>();
            }
            catch (Exception e)
            {
                return DoBadRequestResponse(response, "Input object is not in valid format! It cannot be converted.", e);
            }

            return DoResponse(new EntityManager(service.StorageService).PartialUpdateEntity(entityId, inputEntity));
        }


        // Delete an entity
        // DELETE: api/v1/en_US/meta/entity/{id}
        [AcceptVerbs(new[] { "DELETE" }, Route = "api/v1/en_US/meta/entity/{StringId}")]
        public IActionResult DeleteEntity(string StringId)
        {
            EntityManager manager = new EntityManager(service.StorageService);
            EntityResponse response = new EntityResponse();

            // Parse each string representation.
            Guid newGuid;
            Guid id = Guid.Empty;
            if (Guid.TryParse(StringId, out newGuid))
            {
                response = manager.DeleteEntity(newGuid);
            }
            else
            {
                response.Success = false;
                response.Message = "The entity Id should be a valid Guid";
                Context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
            return DoResponse(response);
        }

        #endregion

        #region << Entity Fields >>
        [AcceptVerbs(new[] { "POST" }, Route = "api/v1/en_US/meta/entity/{Id}/field")]
        public IActionResult CreateField(string Id, [FromBody]JObject submitObj)
        {
            FieldResponse response = new FieldResponse();

            Guid entityId;
            if (!Guid.TryParse(Id, out entityId))
            {
                response.Errors.Add(new ErrorModel("id", Id, "id parameter is not valid Guid value"));
                return DoResponse(response);
            }

            InputField field = new InputGuidField();
            try
            {
                field = InputField.ConvertField(submitObj);
            }
            catch (Exception e)
            {
                return DoBadRequestResponse(response, "Input object is not in valid format! It cannot be converted.", e);
            }

            return DoResponse(new EntityManager(service.StorageService).CreateField(entityId, field));
        }

        [AcceptVerbs(new[] { "PUT" }, Route = "api/v1/en_US/meta/entity/{Id}/field/{FieldId}")]
        public IActionResult UpdateField(string Id, string FieldId, [FromBody]JObject submitObj)
        {
            FieldResponse response = new FieldResponse();

            Guid entityId;
            if (!Guid.TryParse(Id, out entityId))
            {
                response.Errors.Add(new ErrorModel("id", Id, "id parameter is not valid Guid value"));
                return DoResponse(response);
            }

            Guid fieldId;
            if (!Guid.TryParse(FieldId, out fieldId))
            {
                response.Errors.Add(new ErrorModel("id", FieldId, "FieldId parameter is not valid Guid value"));
                return DoResponse(response);
            }

            InputField field = new InputGuidField();

            Type inputFieldType = field.GetType();

            foreach (var prop in submitObj.Properties())
            {
                int count = inputFieldType.GetProperties().Where(n => n.Name.ToLower() == prop.Name.ToLower()).Count();
                if (count < 1)
                    response.Errors.Add(new ErrorModel(prop.Name, prop.Value.ToString(), "Input object contains property that is not part of the object model."));
            }

            if (response.Errors.Count > 0)
                return DoBadRequestResponse(response);

            try
            {
                field = InputField.ConvertField(submitObj);
            }
            catch (Exception e)
            {
                return DoBadRequestResponse(response, "Input object is not in valid format! It cannot be converted.", e);
            }

            return DoResponse(new EntityManager(service.StorageService).UpdateField(entityId, field));
        }

        [AcceptVerbs(new[] { "DELETE" }, Route = "api/v1/en_US/meta/entity/{Id}/field/{FieldId}")]
        public IActionResult DeleteField(string Id, string FieldId)
        {
            FieldResponse response = new FieldResponse();

            Guid entityId;
            if (!Guid.TryParse(Id, out entityId))
            {
                response.Errors.Add(new ErrorModel("id", Id, "id parameter is not valid Guid value"));
                return DoResponse(response);
            }

            Guid fieldId;
            if (!Guid.TryParse(FieldId, out fieldId))
            {
                response.Errors.Add(new ErrorModel("id", FieldId, "FieldId parameter is not valid Guid value"));
                return DoResponse(response);
            }

            return DoResponse(new EntityManager(service.StorageService).DeleteField(entityId, fieldId));
        }


        #endregion

        #region << Relation Meta >>
        // Get all entity relation definitions
        // GET: api/v1/en_US/meta/relation/list/
        [AcceptVerbs(new[] { "GET" }, Route = "api/v1/en_US/meta/relation/list")]
        public IActionResult GetEntityRelationMetaList()
        {
            return DoResponse(new EntityRelationManager(service.StorageService).Read());
        }

        // Get entity relation meta
        // GET: api/v1/en_US/meta/relation/{name}/
        [AcceptVerbs(new[] { "GET" }, Route = "api/v1/en_US/meta/relation/{name}")]
        public IActionResult GetEntityRelationMeta(string name)
        {
            return DoResponse(new EntityRelationManager(service.StorageService).Read(name));
        }


        // Create an entity relation
        // POST: api/v1/en_US/meta/relation
        [AcceptVerbs(new[] { "POST" }, Route = "api/v1/en_US/meta/relation")]
        public IActionResult CreateEntityRelation([FromBody]JObject submitObj)
        {
            try
            {
                if (submitObj["id"].IsNullOrEmpty())
                    submitObj["id"] = Guid.Empty;
                var relation = submitObj.ToObject<EntityRelation>();
                return DoResponse(new EntityRelationManager(service.StorageService).Create(relation));
            }
            catch (Exception e)
            {
                return DoBadRequestResponse(new EntityRelationResponse(), null, e);
            }
        }

        // Update an entity relation
        // PUT: api/v1/en_US/meta/relation/id
        [AcceptVerbs(new[] { "PUT" }, Route = "api/v1/en_US/meta/relation/{RelationIdString}")]
        public IActionResult UpdateEntityRelation(string RelationIdString, [FromBody]JObject submitObj)
        {
            FieldResponse response = new FieldResponse();

            Guid relationId;
            if (!Guid.TryParse(RelationIdString, out relationId))
            {
                response.Errors.Add(new ErrorModel("id", RelationIdString, "id parameter is not valid Guid value"));
                return DoResponse(response);
            }

            try
            {
                var relation = submitObj.ToObject<EntityRelation>();
                return DoResponse(new EntityRelationManager(service.StorageService).Update(relation));
            }
            catch (Exception e)
            {
                return DoBadRequestResponse(new EntityRelationResponse(), null, e);
            }
        }

        // Delete an entity relation
        // DELETE: api/v1/en_US/meta/relation/{idToken}
        [AcceptVerbs(new[] { "DELETE" }, Route = "api/v1/en_US/meta/relation/{idToken}")]
        public IActionResult DeleteEntityRelation(string idToken)
        {
            Guid newGuid;
            Guid id = Guid.Empty;
            if (Guid.TryParse(idToken, out newGuid))
            {
                return DoResponse(new EntityRelationManager(service.StorageService).Delete(newGuid));
            }
            else
            {
                return DoBadRequestResponse(new EntityRelationResponse(), "The entity relation Id should be a valid Guid", null);
            }

        }

        #endregion

        #region << Records >>
        // Create an entity record
        // POST: api/v1/en_US/record/{entityName}
        [AcceptVerbs(new[] { "POST" }, Route = "api/v1/en_US/record/{entityName}")]
        public IActionResult CreateEntityRecord(string entityName, [FromBody]EntityRecord postObj)
        {
            QueryResponse result = recMan.CreateRecord(entityName, postObj);
            if (!result.Success)
                return DoResponse(result);
            return Json(result);
        }

        // Update an entity record
        // PUT: api/v1/en_US/record/{entityName}/{recordId}
        [AcceptVerbs(new[] { "PUT" }, Route = "api/v1/en_US/record/{entityName}/{recordId}")]
        public IActionResult UpdateEntityRecord(string entityName,Guid recordId, [FromBody]EntityRecord postObj)
        {
            QueryResponse result = recMan.UpdateRecord(entityName, postObj);
            if (!result.Success)
                return DoResponse(result);
            return Json(result);
        }

        // Patch an entity record
        // PATCH: api/v1/en_US/record/{entityName}/{recordId}
        [AcceptVerbs(new[] { "PATCH" }, Route = "api/v1/en_US/record/{entityName}/{recordId}")]
        public IActionResult PatchEntityRecord(string entityName, Guid recordId, [FromBody]EntityRecord postObj)
        {
            QueryResponse result = recMan.UpdateRecord(entityName, postObj);
            if (!result.Success)
                return DoResponse(result);
            return Json(result);
        }

        // Get an entity record list
        // GET: api/v1/en_US/record/{entityName}/list
        [AcceptVerbs(new[] { "GET" }, Route = "api/v1/en_US/record/{entityName}/list")]
        public IActionResult GetRecordsByEntityName(string entityName)
        {

            QuerySortObject sortObj = new QuerySortObject("label", QuerySortType.Ascending);

            EntityQuery query = new EntityQuery(entityName,"*",null, new[] { sortObj },null,null);

            QueryResponse result = recMan.Find(query);
            if (!result.Success)
                return DoResponse(result);
            return Json(result);
        }



        #endregion

        //#region << Area Specific >>

        // Delete an area record
        // DELETE: api/v1/en_US/area/{recordId}
        [AcceptVerbs(new[] { "DELETE" }, Route = "api/v1/en_US/area/{recordId}")]
        public IActionResult DeleteAreaRecord(Guid recordId)
        {
            QueryResponse response = new QueryResponse();
            //Begin transaction
            var recRep = Storage.GetRecordRepository();
            var transaction = recRep.CreateTransaction();
            try
            {
                transaction.Begin();
                //Delete all relations in the areas_entities collection/entity
                List<EntityRecord> areasEntititesRelations = new List<EntityRecord>();
                QueryObject areasEntititesRelationsFilterObj = EntityQuery.QueryEQ("area_id", recordId);
                var areasEntititesRelationsQuery = new EntityQuery("areas_entities", "*", areasEntititesRelationsFilterObj, null, null, null);
                var areasEntititesRelationsResult = recMan.Find(areasEntititesRelationsQuery);
                if (!areasEntititesRelationsResult.Success) {
                    response.Timestamp = DateTime.UtcNow;
                    response.Success = false;
                    response.Message = areasEntititesRelationsResult.Message;
                    transaction.Rollback();
                    return Json(response);
                }
                if (areasEntititesRelationsResult.Object.Data != null && areasEntititesRelationsResult.Object.Data.Any())
                {
                    areasEntititesRelations = areasEntititesRelationsResult.Object.Data;
                }
                foreach (var relation in areasEntititesRelations)
                {
                    var relationDeleteResult = recMan.DeleteRecord("areas_entities", (Guid)relation["id"]);
                    if (!relationDeleteResult.Success)
                    {
                        response.Timestamp = DateTime.UtcNow;
                        response.Success = false;
                        response.Message = relationDeleteResult.Message;
                        transaction.Rollback();
                        return Json(response);
                    }
                }

                //Delete the area
                var areaDeleteResult = recMan.DeleteRecord("area", recordId);
                if (!areaDeleteResult.Success)
                {
                    response.Timestamp = DateTime.UtcNow;
                    response.Success = false;
                    response.Message = areaDeleteResult.Message;
                    transaction.Rollback();
                    return Json(response);
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }

            response.Timestamp = DateTime.UtcNow;
            response.Success = true;
            response.Message = "Area successfully deleted";
            return DoResponse(response);
         }

        //#endregion


    }
}

