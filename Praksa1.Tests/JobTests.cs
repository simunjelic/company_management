using Praksa_projectV1.DataAccess;
using Praksa_projectV1.Models;
using Praksa_projectV1.ViewModels;
using Xunit;
using Moq; // You'll need to install the Moq package
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Autofac.Extras.Moq;
using System.Windows.Forms;

namespace Praksa1.Tests
{
    public class JobTests
    {


        [Fact]
        public async Task AddJobAsync_ValidData_JobAddedSuccessfully()
        {
            // Arrange
            var viewModel = new JobsViewModel();
            var fakeRepository = new Mock<IJobRepository>();
            fakeRepository.Setup(repo => repo.AddJobAsync(It.IsAny<Job>())).ReturnsAsync(true);
            viewModel.repository = fakeRepository.Object;
            viewModel.AddName = "Serviser";
            // Assuming department with Id 1 exists for testing purposes
            viewModel.SelectedDepartment = new Department { Id = 4, Name = "Servis" };
            viewModel.JobRecords = new ObservableCollection<Job>();

            // Act
            await viewModel.AddJobAsync();

            // Assert
            // You can assert whether the job was added to the repository or not
            // For example, check if the JobRecords collection contains the newly added job
            Assert.Contains(viewModel.JobRecords, job => job.Name == "Serviser");
        }
        [Fact]
        public async Task DeleteJobAsync_ValidData_JobDeletedSuccessfully()
        {
            // Arrange
            // Assuming you have a selected job to delete
            var viewModel = new JobsViewModel();
            var fakeRepository = new Mock<IJobRepository>();
            var jobs = new List<Job>
    {
        new Job { Id = 1, Name = "Job 1" },
        new Job { Id = 2, Name = "Job 2" }
    };
            fakeRepository.Setup(repo => repo.GetAllJobsAsync()).ReturnsAsync(jobs);
            fakeRepository.Setup(repo => repo.RemoveJob(It.IsAny<Job>())).ReturnsAsync(true);
            viewModel.repository = fakeRepository.Object;
            viewModel.JobRecords = new ObservableCollection<Job>();
            await viewModel.GetAll();
            viewModel.SelectedJob = viewModel.JobRecords.LastOrDefault();
            var idCheck = viewModel.SelectedJob.Id;

            // Act
            await viewModel.DeleteJobAsync();

            // Assert
            // Verify that the RemoveJob method was called with the correct job ID
            // Verify that the job was removed from the ViewModel's JobRecords collection
            Assert.DoesNotContain(viewModel.JobRecords, job => job.Id == idCheck);
        }

        
    }
}