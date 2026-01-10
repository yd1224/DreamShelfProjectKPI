import type { H3Event } from 'h3'

export const forwardCookies = (response: Response | undefined, event: H3Event) => {
    if (!response) return;
    const cookies =
        response.headers.getSetCookie?.() ||
        response.headers.raw?.()['set-cookie'] ||
        []
    const cookiesArr = Array.isArray(cookies) ? cookies : [cookies]

    for (const c of cookiesArr) {
        if (c) {
            // appendHeader preserves multiple cookies
            event.node.res.appendHeader('Set-Cookie', c)
        }
    }
}
