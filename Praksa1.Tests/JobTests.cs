using Moq;
using Praksa_projectV1.DataAccess;
using Praksa_projectV1.Models;
using Praksa_projectV1.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xunit;

namespace Praksa1.Tests
{
    public class JobTests
    {


        private readonly Mock<IJobRepository> _mockJobRepository;
        private readonly Mock<IDepartmentRepository> _mockDepartmentRepository;
        private readonly JobsViewModel _viewModel;

        public JobTests()
        {
            _mockJobRepository = new Mock<IJobRepository>();
            _mockDepartmentRepository = new Mock<IDepartmentRepository>();

            _viewModel = new JobsViewModel()
            {
                repository = _mockJobRepository.Object,
                departmentRepository = _mockDepartmentRepository.Object,
                JobRecords = new ObservableCollection<Job>(),
                DepartmentRecords = new ObservableCollection<Department>()
            };
        }

        [Fact]
        public async Task AddJobAsync_ShouldAddJob_WhenValid()
        {
            // Arrange
            _viewModel.AddName = "New Job";
            var department = new Department { Id = 1, Name = "Department 1" };
            _viewModel.SelectedDepartment = department;

            _mockJobRepository.Setup(repo => repo.AddJobAsync(It.IsAny<Job>()))
                .ReturnsAsync(true);

            // Act
            await _viewModel.AddJobAsync();

            // Assert
            _mockJobRepository.Verify(repo => repo.AddJobAsync(It.Is<Job>(job => job.Name == "New Job" && job.DepartmentId == 1)), Times.Once);
            Assert.Contains(_viewModel.JobRecords, job => job.Name == "New Job" && job.DepartmentId == 1);
        }

        [Fact]
        public async Task DeleteJobAsync_ShouldDeleteJob_WhenSelectedJobIsNotNull()
        {
            // Arrange
            var job = new Job { Id = 1, Name = "Job 1", DepartmentId = 1 };
            _viewModel.SelectedJob = job;
            _viewModel.JobRecords.Add(job);

            _mockJobRepository.Setup(repo => repo.RemoveJob(It.IsAny<Job>()))
                .ReturnsAsync(true);

            // Act
            await _viewModel.DeleteJobAsync();

            // Assert
            _mockJobRepository.Verify(repo => repo.RemoveJob(It.Is<Job>(j => j.Id == 1)), Times.Once);
            Assert.DoesNotContain(_viewModel.JobRecords, j => j.Id == 1);
        }

        [Fact]
        public async Task GetAll_ShouldPopulateJobRecords()
        {
            // Arrange
            var jobs = new List<Job>
            {
                new Job { Id = 1, Name = "Job 1", DepartmentId = 1 },
                new Job { Id = 2, Name = "Job 2", DepartmentId = 2 }
            };
            _mockJobRepository.Setup(repo => repo.GetAllJobsAsync()).ReturnsAsync(jobs);

            // Act
            await _viewModel.GetAll();

            // Assert
            Assert.Equal(2, _viewModel.JobRecords.Count);
        }

        [Fact]
        public async Task GetAllDepartmentsAsync_ShouldPopulateDepartmentRecords()
        {
            // Arrange
            var departments = new ObservableCollection<Department>
            {
                new Department { Id = 1, Name = "Department 1" },
                new Department { Id = 2, Name = "Department 2" }
            };
            _mockDepartmentRepository.Setup(repo => repo.GetAllDepartmentsAsync()).ReturnsAsync(departments);

            // Act
            await _viewModel.GetAllDepartmentsAsync();

            // Assert
            Assert.Equal(2, _viewModel.DepartmentRecords.Count);
        }


    }
}