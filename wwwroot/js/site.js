// Global app configuration
const App = {
    config: {
        pageSize: 10,
        searchDelay: 300
    },

    // Initialize the application
    init: function () {
        this.setupEventHandlers();
        this.initializeComponents();
    },

    // Setup global event handlers
    setupEventHandlers: function () {
        // Bootstrap tooltip initialization
        this.initializeTooltips();

        // Form validation enhancements
        this.enhanceFormValidation();

        // Table enhancements
        this.enhanceTableFeatures();

        // Search functionality
        this.initializeSearch();
    },

    // Initialize Bootstrap tooltips
    initializeTooltips: function () {
        const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
        tooltipTriggerList.map(function (tooltipTriggerEl) {
            return new bootstrap.Tooltip(tooltipTriggerEl);
        });
    },

    // Enhance form validation
    enhanceFormValidation: function () {
        // Add custom validation styles
        const forms = document.querySelectorAll('.needs-validation');
        Array.prototype.slice.call(forms).forEach(function (form) {
            form.addEventListener('submit', function (event) {
                if (!form.checkValidity()) {
                    event.preventDefault();
                    event.stopPropagation();
                }
                form.classList.add('was-validated');
            }, false);
        });

        // Email validation enhancement
        const emailInputs = document.querySelectorAll('input[type="email"]');
        emailInputs.forEach(function (input) {
            input.addEventListener('blur', function () {
                App.validateEmail(this);
            });
        });

        // Salary validation
        const salaryInputs = document.querySelectorAll('input[name="Salary"]');
        salaryInputs.forEach(function (input) {
            input.addEventListener('input', function () {
                App.formatSalary(this);
            });
        });
    },

    // Email validation
    validateEmail: function (input) {
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        const isValid = emailRegex.test(input.value);

        if (input.value && !isValid) {
            input.setCustomValidity('Please enter a valid email address');
        } else {
            input.setCustomValidity('');
        }
    },

    // Format salary input
    formatSalary: function (input) {
        let value = input.value.replace(/[^\d.]/g, '');
        if (value && !isNaN(value)) {
            // Ensure only two decimal places
            const parts = value.split('.');
            if (parts[1] && parts[1].length > 2) {
                value = parts[0] + '.' + parts[1].substring(0, 2);
            }
            input.value = value;
        }
    },

    // Enhance table features
    enhanceTableFeatures: function () {
        // Add row hover effects
        const tableRows = document.querySelectorAll('.table tbody tr');
        tableRows.forEach(function (row) {
            row.addEventListener('mouseenter', function () {
                this.classList.add('table-active');
            });
            row.addEventListener('mouseleave', function () {
                this.classList.remove('table-active');
            });
        });
    },

    // Initialize search functionality
    initializeSearch: function () {
        const searchInputs = document.querySelectorAll('.search-input');
        searchInputs.forEach(function (input) {
            let searchTimeout;
            input.addEventListener('input', function () {
                clearTimeout(searchTimeout);
                searchTimeout = setTimeout(function () {
                    App.performSearch(input.value);
                }, App.config.searchDelay);
            });
        });
    },

    // Perform search operation
    performSearch: function (term) {
        if (term.length < 2) return;

        // Implementation depends on the page context
        console.log('Searching for:', term);
    },

    // Initialize page-specific components
    initializeComponents: function () {
        // Dashboard charts
        if (document.getElementById('departmentChart')) {
            this.initializeDashboardCharts();
        }

        // Confirmation dialogs
        this.initializeConfirmationDialogs();
    },

    // Initialize confirmation dialogs
    initializeConfirmationDialogs: function () {
        const deleteButtons = document.querySelectorAll('[data-confirm]');
        deleteButtons.forEach(function (button) {
            button.addEventListener('click', function (e) {
                const message = this.getAttribute('data-confirm') || 'Are you sure?';
                if (!confirm(message)) {
                    e.preventDefault();
                    return false;
                }
            });
        });
    },

    // Utility functions
    utils: {
        // Show loading spinner
        showLoading: function (element) {
            element.innerHTML = '<div class="d-flex justify-content-center"><div class="spinner-border" role="status"><span class="visually-hidden">Loading...</span></div></div>';
        },

        // Hide loading spinner
        hideLoading: function (element) {
            const spinner = element.querySelector('.spinner-border');
            if (spinner) {
                spinner.parentElement.parentElement.remove();
            }
        },

        // Format currency
        formatCurrency: function (amount) {
            return new Intl.NumberFormat('en-US', {
                style: 'currency',
                currency: 'USD'
            }).format(amount);
        },

        // Format date
        formatDate: function (date) {
            return new Intl.DateTimeFormat('en-US', {
                year: 'numeric',
                month: 'short',
                day: 'numeric'
            }).format(new Date(date));
        },

        // Debounce function
        debounce: function (func, wait) {
            let timeout;
            return function executedFunction(...args) {
                const later = () => {
                    clearTimeout(timeout);
                    func(...args);
                };
                clearTimeout(timeout);
                timeout = setTimeout(later, wait);
            };
        }
    }
};

// Initialize app when DOM is loaded
document.addEventListener('DOMContentLoaded', function () {
    App.init();
});

// Export for global use
window.App = App;