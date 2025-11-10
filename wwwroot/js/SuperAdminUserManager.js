class UserManagerSearch {
    constructor(inputField, outputField, organizationSelector) {
        this._inputField = inputField;
        this._outputField = outputField;
        this._organizationSelector = organizationSelector;
        this._users = [];
        this._organizations = [];
    }
    async Init() {
        await this.FetchUsers("");
        this.ViewUsers();

        this._inputField.addEventListener('keyup', () => this.UpdateFromField());
        this._organizationSelector.addEventListener('change', () => this.ViewUsers());
        this._outputField.addEventListener("click", (e) => {
            let row = e.target.closest("tr.user-row"); // find clicked row
            if (!row) return; // clicked outside row

            const userId = row.dataset.userId;
            window.location.href = `/Superadmin/ManageUsers/${userId}`;
        });

    }
    async UpdateFromField() {
        await this.FetchUsers(this._inputField.value);
        this.ViewUsers();
    }
    async FetchUsers(FullName) {
        const resp = await fetch(`/Superadmin/FetchUsers?FullName=${FullName}`);
        const respJson = await resp.json();
        this._users = respJson.data || [];
        this._organizations = [...new Set(UMS._users.map(u => u.organization))];
        this.UpdateSelector();

    }
    UpdateSelector() {
        let selectorOptionsHtml = '<option value="fhikdknflsm343443">All</option>';
        this._organizations.forEach((org) => { selectorOptionsHtml += `<option value=${org}>${org}</option>`});

        this._organizationSelector.innerHTML = selectorOptionsHtml;
    }
    ViewUsers() {
        
        let htmlstring = `<table class="UserTable">
            <tr><td>Full name</td>
            <td>Organization</td>
            <td>Email</td>
            <td>Role</td>
            <td>Register date</td></tr>
        
        `;
        let UserToViewFiltered;
        if (this._organizationSelector.value != "fhikdknflsm343443") {
            UserToViewFiltered = this._users.filter((u) => u.organization.toLowerCase() == this._organizationSelector.value.toLowerCase())
        } else {

            UserToViewFiltered = this._users;
        }

        UserToViewFiltered.forEach((user) => { htmlstring += this.CreateUserContainerElement(user) });
        htmlstring += "</table>"
        this._outputField.innerHTML = htmlstring;
    }
    CreateUserContainerElement(user) {
        return `
        <tr class="user-row" data-user-id="${user.id}">
            <td>${user.firstName} ${user.lastName}</td>
            <td>${user.organization}</td>
            <td>${user.email}</td>
            <td>${user.userType}</td>
            <td>${new Date(user.createdAt).toLocaleString()}</td>
        </tr>
    `;
    }

}
class UserManagerPage {
    constructor(userId, roleSetUser, roleSetAdmin, roleSetEmployee, deleteButton, csrfToken) {
        this._roleSetUser = roleSetUser;
        this._roleSetAdmin = roleSetAdmin;
        this._deleteButton = deleteButton;
        this._userId = userId;
        this._roleSetEmployee = roleSetEmployee;
        this._csrfToken = csrfToken;

        this._roleSetAdmin.addEventListener('click', () => this.SetRole('Admin'))
        this._roleSetUser.addEventListener('click', () => this.SetRole('User'))
        this._roleSetEmployee.addEventListener('click', () => this.SetRole('Employee'))
        this._deleteButton.addEventListener('click', () => this.DeleteUser())

    }
    async SetRole(role) {
        await fetch(`/Superadmin/SetUserRole`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'X-CSRF-TOKEN': this._csrfToken
            },
            body: JSON.stringify({ Id: this._userId, UserType: role })
        });
        window.location.reload();
    }
    async DeleteUser() {
        await fetch(`/Superadmin/DeleteUser`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'X-CSRF-TOKEN': this._csrfToken
            },
            body: JSON.stringify({ Id: this._userId })
        });
        window.location.href = '/';

    }
}