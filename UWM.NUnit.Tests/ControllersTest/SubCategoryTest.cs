﻿using Newtonsoft.Json;
using NUnit.Framework;
using System.Net.Http.Json;
using System.Net;
using UWM.Domain.DTO.Category;
using UWM.Domain.DTO.SubCategory;
using UWM.Domain.Entity;

namespace UWM.NUnit.Tests.ControllersTest
{
    public class SubCategoryTest
    {
        private int id;
        private readonly HttpClient client = ClientForTests.GetClient();

        [Test]
        public async Task A_Post_ShouldBeOkAndIsNotNull()
        {
            // Arrange
            var subCategory = new SubCategoryDto
            {
                Name = "Test",
                CategoryId = 1,
            };
            string json = JsonConvert.SerializeObject(subCategory);
            StringContent httpContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            // Act
            HttpResponseMessage response = await client.PostAsync("api/SubCategory", httpContent);
            var res = await response.Content.ReadFromJsonAsync<int>();
            id = res;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsTrue(res > 0);
        }

        [Test]
        public async Task B_GetAll_ShouldBeOkAndIsNotNull()
        {
            // Arrange

            // Act
            HttpResponseMessage response = await client.GetAsync("api/SubCategory");
            var res = await response.Content.ReadFromJsonAsync<IEnumerable<CategoryDto>>();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsNotNull(res.FirstOrDefault(), "subCategory is Null");
        }

        [Test]
        public async Task E_Put_ShouldBeOk()
        {
            // Arrange
            var subCategory = new SubCategory
            {
                Id = id,
                Name = "Обновлен",
                CategoryId = 1,
            };
            string json = JsonConvert.SerializeObject(subCategory);
            StringContent httpContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            // Act
            HttpResponseMessage response = await client.PutAsync($"api/SubCategory/{id}", httpContent);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [Test]
        public async Task F_Delete_ShouldBeOk()
        {
            // Arrange

            // Act
            HttpResponseMessage response = await client.DeleteAsync($"api/SubCategory/{id}");

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
