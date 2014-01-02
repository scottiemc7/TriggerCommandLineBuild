package triggercommandlinebuild.agent;

import jetbrains.buildServer.RunBuildException;
import jetbrains.buildServer.agent.AgentRunningBuild;
import jetbrains.buildServer.agent.artifacts.ArtifactsWatcher;
import jetbrains.buildServer.agent.runner.CommandLineBuildService;
import jetbrains.buildServer.agent.runner.SimpleProgramCommandLine;
import triggercommandlinebuild.common.iPluginConstants;
import java.util.ArrayList;
import java.util.List;
import java.util.Map;

public class TSAService extends CommandLineBuildService {
    private final ArtifactsWatcher artifactsWatcher;

    public TSAService(ArtifactsWatcher artifactsWatcher) {
        this.artifactsWatcher = artifactsWatcher;
    }

    @Override
    public jetbrains.buildServer.agent.runner.ProgramCommandLine makeProgramCommandLine() throws RunBuildException {
        AgentRunningBuild build = getBuild();

        List<String> args = createArgs();

        return new SimpleProgramCommandLine(build,
                build.getAgentConfiguration().getAgentPluginsDirectory() +  "\\" + iPluginConstants.PLUGIN_TYPE + "\\TriggerCommandLineConsole.exe",
                args);
    }

    private List<String> createArgs() {
        Map<String,String> parameters = getBuild().getRunnerParameters();
        List<String> result = new ArrayList<String>();

        result.add("--email=" + parameters.get(iPluginConstants.PROPERTYKEY_EMAIL));
        result.add("--password=" + parameters.get(iPluginConstants.PROPERTYKEY_PASSWORD));
        result.add("--src=" + getBuild().getCheckoutDirectory() + checkForNull(parameters.get(iPluginConstants.PROPERTYKEY_SRCPATH)));
        result.add("--package=" + getBuild().getCheckoutDirectory());
        result.add("--forge=" + parameters.get(iPluginConstants.PROPERTYKEY_FORGEPATH));
        result.add("--sdk=" + parameters.get(iPluginConstants.PROPERTYKEY_ANDROIDSDKATH));
        String check = parameters.get(iPluginConstants.PROPERTYKEY_SANITYCHECK);
        if(check != null && check.compareToIgnoreCase("true") == 0)
            result.add("--check");
        String android = parameters.get(iPluginConstants.PROPERTYKEY_PLATFORM_ANDROID);
        if(android != null && android.compareToIgnoreCase("true") == 0) {
            result.add("--android");
            result.add("--keystore=" + parameters.get(iPluginConstants.PROPERTYKEY_ANDROIDKEYSTOREPATH));
            result.add("--keystorepass=" + parameters.get(iPluginConstants.PROPERTYKEY_ANDROIDKEYSTOREPASSWORD));
            result.add("--keyalias=" + parameters.get(iPluginConstants.PROPERTYKEY_ANDROIDKEYALIAS));
            result.add("--keypass=" + parameters.get(iPluginConstants.PROPERTYKEY_ANDROIDKEYPASSWORD));
            /*
            String ignore = parameters.get(iPluginConstants.PROPERTYKEY_ANDROIDIGNOREPATH);
            if(ignore != null && ignore.compareToIgnoreCase("null") != 0 && ignore.length() > 0)
                result.add("--androidignore=\"" + parameters.get(iPluginConstants.PROPERTYKEY_ANDROIDIGNOREPATH) + "\"");
            */
        }//end if

        String ios = parameters.get(iPluginConstants.PROPERTYKEY_PLATFORM_IOS);
        if(ios != null && ios.compareToIgnoreCase("true") == 0) {
            result.add("--ios");
            result.add("--cert=" + parameters.get(iPluginConstants.PROPERTYKEY_IOSCERTIFICATEPATH));
            result.add("--certpass=" + parameters.get(iPluginConstants.PROPERTYKEY_IOSCERTIFICATEPASSWORD));
            result.add("--profile=" + parameters.get(iPluginConstants.PROPERTYKEY_IOSPROFILEPATH));
            /*
            String ignore = parameters.get(iPluginConstants.PROPERTYKEY_IOSIGNOREPATH);
            if(ignore != null && ignore.compareToIgnoreCase("null") != 0 && ignore.length() > 0)
                result.add("--iosignore=\"" + parameters.get(iPluginConstants.PROPERTYKEY_IOSIGNOREPATH) + "\"");
            */
        }//end if

        return result;
    }

    private String checkForNull(String str)
    {
        if(str == null || str.length() == 0 || str.compareToIgnoreCase("null") == 0)
            return "";
        else
            return str;
    }
}

