import { UserManager, WebStorageStateStore} from "oidc-client-ts";

const userManager = new UserManager({
    authority: 'http://localhost:8080/realms/oidc-lab',
    client_id: 'oidc-lab-vue',
    redirect_uri: 'http://localhost:5173/auth-callback',
    post_logout_redirect_uri: 'http://localhost:5173',
    response_type: 'code',
    scope: 'openid profile email',
    userStore: new WebStorageStateStore({ store: window.sessionStorage }),
    automaticSilentRenew: true,
})

export default userManager;