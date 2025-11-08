class Notifications {
    constructor(NotificationBoxElement, NotificationButtonElement) {
        this._nbe = NotificationBoxElement;
        this._NotificationButton = NotificationButtonElement;
        this._Notifications = []

        this.Init();
    }
    async Init() {
        await this.GetNotifications();
        this.SetupNotificationButton(this.GetUnreadAmount());
    }
        

    async GetNotifications() {
        try {
            const response = await fetch('/api/GetNotifications');
            const data = await response.json();

            if (data.success) {
                this._Notifications = data.data; 
                return data.data; 
            } else {
                this._Notifications = [];
                return [];
            }
        } catch (err) {
            console.error("Failed to fetch notifications:", err);
            this._Notifications = [];
            return [];
        }
    }
    CreateNotificationElement(Notification) {
        if (Notification.markerId == 0) {
            return `
            <div id="notification-id-${Notification.id}" class='notification-container notification-is-read-${Notification.isRead}'>
            <p>${Notification.message}</p>
            <button class="notification-delete-button" id="notification-delete-id-${Notification.id}">X</button>
            </div>
            `;
        } else {
            return `
            <div id="notification-id-${Notification.id}" class='notification-container notification-is-read-${Notification.isRead}'>
            <p>${Notification.message}</p>
            <a href="/ViewMarker/${Notification.markerId}"><button>View Submission ${Notification.markerId}</button></a>
            <button class="notification-delete-button" id="notification-delete-id-${Notification.id}">X</button>
            </div>
            `;
        }
        
    }
    async SendViewedState(nid) {
        await fetch(`/api/MarkNotificationAsRead?NotificationId=${nid}`);
        await this.GetNotifications();
        this.ShooshNotificationsToBox();
        this.SetupNotificationButton();
    }
    async DeleteNotification(nid) {
        await fetch(`/api/DeleteNotification?NotificationId=${nid}`);
        await this.GetNotifications();
        this.ShooshNotificationsToBox();
        this.SetupNotificationButton();
    }
    ShooshNotificationsToBox() {
        let htmlString = "<button id='notification-close-button'>Close</button>";
        this._Notifications.forEach(notification => {
            htmlString += this.CreateNotificationElement(notification);
        }) 
        this._nbe.innerHTML = htmlString;

        this._Notifications.forEach(notification => {
            const noti = this._nbe.querySelector(`#notification-id-${notification.id}`);
            if (noti) {
                noti.addEventListener('click', () => this.SendViewedState(notification.id));
            }
            noti.querySelector(".notification-delete-button").addEventListener('click', () => {
                this.DeleteNotification(notification.id);
            })
        });
        this._nbe.querySelector('#notification-close-button').addEventListener('click', () => {
            this._Notifications = [];
            this._nbe.innerHTML = "";
            this._nbe.style.display = "none";
        });
    }
    async OpenNotificationBox() {
        await this.GetNotifications();
        this.ShooshNotificationsToBox();
        this._nbe.style.display = "inline-block";
    }
    GetUnreadAmount() {
        try {
            let l = this._Notifications.filter((noti) => !noti.isRead).length;
            if (l > 0) {
                return l;
            }
            return "";
        } catch {
            return "";
        }
        
    }

    SetupNotificationButton(notificationAmount) {

        this._NotificationButton.addEventListener('click', () => { this.OpenNotificationBox() })
        if (notificationAmount == undefined) notificationAmount = this.GetUnreadAmount();
        this._NotificationButton.innerHTML = "🔔" + notificationAmount;
    }
}
