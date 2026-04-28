ALTER TABLE Products
ADD 
    MonthlyInstallmentAmount DECIMAL(18,2) NULL,
    NoOfInstallment INT NULL;

ALTER TABLE SaleLines
ADD 
    MonthlyInstallmentAmount DECIMAL(18,2) NULL,
    NoOfInstallment INT NULL,
    FirstInstallmentDate DATE NULL;