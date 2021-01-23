#import <Foundation/Foundation.h>
#import "NativeCallProxy.h"


@implementation FrameworkLibAPI

id<NativeCallsProtocol> api = NULL;
+(void) registerAPIforNativeCalls:(id<NativeCallsProtocol>) aApi
{
    api = aApi;
}

@end


extern "C" {
    void InitializeApi(const char* apiGameObject) {
        assert(api != nil);
        [api initializeApi:[NSString stringWithUTF8String:apiGameObject]];
    }

    void OnLoadingSucceeded() {
        assert(api != nil);
        [api onLoadingSucceeded];
    }

    void OnLoadingFailed() {
        assert(api != nil);
        [api onLoadingFailed];
    }

    void OnUnloadingSucceeded() {
        assert(api != nil);
        [api onUnloadingSucceeded];
    }

    void OnSaveDataWritten() {
        assert(api != nil);
        [api onSaveDataWritten];
    }

    void OnGameExitRequested() {
        assert(api != nil);
        [api onGameExitRequested];
    }

    void OnCreateScreenshotFailed() {
        assert(api != nil);
        [api onCreateScreenshotFailed];
    }

    void OnCreateScreenshotSucceeded(const char* apiGameObject) {
        assert(api != nil);
        [api onCreateScreenshotSucceeded:[NSString stringWithUTF8String:apiGameObject]];
    }
}

