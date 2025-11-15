class SuperAdminReviewAssign {
    MarkerArray = [];
    CheckBoxes = [];
    Employees = [];
    OutputDiv = document.getElementById('SARA-OUTPUT');
    EmployeeSelect = document.getElementById('SARA-SELECT');
    Constructor(CSRF) {
        this.csrf = CSRF;
    }

    async Init() {
        await this.SetupTable();
        await this.SetupSelector();
        window.SARA = this;
    }

    async FetchMarkers() {
        try {
            const resp = await fetch("/Superadmin/FetchUnassignedMarkers");
            const data = await resp.json();
            this.MarkerArray = data.data;
        } catch {
            this.MarkerArray = [];
        }
    }
    async FetchEmployees() {
        try {
            const resp = await fetch("/Superadmin/FetchEmployees");
            const data = await resp.json();
            this.Employees = data.data;
        } catch {
            this.Employees = [];
        }
    }
    async SetupTable() {
        await this.FetchMarkers();
        let htmlString = `<table class="UserTable">
            <tr>
                <td>Type</td>
                <td>Category</td>
                <td>Organization</td>
                <td>Source</td>
                <td>Height (Meters)</td>
                <td>Checkboxes</td>
                <td>Assign to selected</td>
            </tr>`;
        this.MarkerArray.forEach(mrk => htmlString += this.CreateTableRow(mrk));
        htmlString += `</table>`;
        this.OutputDiv.innerHTML = htmlString;
        this.CheckBoxes = Array.from(this.OutputDiv.querySelectorAll(".checkbox-submission-assign"));

    }
    GetCheckedIds() {
        let OnlyChecked = this.CheckBoxes.filter(el => el.checked);
        let ids = OnlyChecked.map(el => Number(el.id.split('-')[1]));
        return ids;
    }
    GetAvailableIds() {
        let ids = this.CheckBoxes.map(el => Number(el.id.split('-')[1]));
        return ids;
    }

    CreateTableRow(mrk) {
        return `
            <tr>
                <td>${mrk.type}</td>
                <td>${mrk.obstacleCategory}</td>
                <td>${mrk.organization}</td>
                <td>${mrk.source}</td>
                <td>${mrk.heightM} m</td>
                <td><input class="checkbox-submission-assign" id="submisstion-${mrk.markerId}" type="checkbox" /></td>
                <td><button onclick="window.SARA.PostQuickAssign(${mrk.markerId})">Quick assign</button></td>
            </tr>
            `;
    }
    CreateOption(user) {
        return `<option value=${user.id}>${user.firstName} ${user.lastName}</option>`;
    }
    async SetupSelector() {
        await this.FetchEmployees();
        let htmlString;
        this.Employees.forEach(usr => htmlString += this.CreateOption(usr));
        this.EmployeeSelect.innerHTML = htmlString;
    }

    async PostBulkAssign() {
        const ids = this.GetCheckedIds();
        const SelectedUser = Number(this.EmployeeSelect.value);
        const RARRAY = ids.map(id => new ReviewAssign(SelectedUser, id));

        console.log(RARRAY);
        const resp = await fetch("/Superadmin/PostAssignReviews", {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "RequestVerificationToken": this.csrf
            },
            body: JSON.stringify(RARRAY)
        })
        const gresp = await resp.json();
        console.log(gresp);
        if (gresp.success) {
            await this.SetupTable();
        }

        return gresp;
    }
    async PostQuickAssign(mrkId) {
        const ids = [mrkId];
        const SelectedUser = Number(this.EmployeeSelect.value);
        const RARRAY = ids.map(id => new ReviewAssign(SelectedUser, id));

        console.log(RARRAY);
        const resp = await fetch("/Superadmin/PostAssignReviews", {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "RequestVerificationToken": this.csrf
            },
            body: JSON.stringify(RARRAY)
        })
        const gresp = await resp.json();
        console.log(gresp);
        if (gresp.success) {
            await this.SetupTable();
        }

        return gresp;
    }
    async AutoAssign() {
        console.log("shit");
        // markers / employess
        //
        const Mlength = this.GetAvailableIds().length;
        const Elength = this.Employees.length;
        const MarkersPerEmployee = Math.ceil(Mlength / Elength);

        for (const e of this.Employees) {
            this.EmployeeSelect.value = e.id;
            for (let i = 0; i < MarkersPerEmployee; i++) {
                try {
                    this.CheckBoxes[i].checked = true;
                } catch {

                }
            }
            await this.PostBulkAssign();
        }

    }

}
class ReviewAssign {
    constructor(uid, mid) { 
        this.UserId = uid;
        this.MarkerId = mid;
    }
}

