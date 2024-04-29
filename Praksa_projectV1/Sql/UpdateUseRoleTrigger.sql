CREATE TRIGGER UpdateUserRoles
ON user_roles
AFTER INSERT, DELETE
AS
BEGIN
    DECLARE @UserID INT;

    -- Get the user ID affected by the insert or delete operation
    IF EXISTS (SELECT * FROM inserted)
    BEGIN
        SELECT @UserID = UserID FROM inserted;
    END
    ELSE
    BEGIN
        SELECT @UserID = UserID FROM deleted;
    END

    -- Concatenate the role names associated with the user
    DECLARE @RoleNames VARCHAR(MAX);
    SELECT @RoleNames = STRING_AGG(r.roleName, ', ')
    FROM user_roles ur
    INNER JOIN roles r ON ur.RoleID = r.id
    WHERE ur.UserID = @UserID;

    -- Update the roles column in the users table
    UPDATE users
    SET roles = @RoleNames
    WHERE id = @UserID;
END;
